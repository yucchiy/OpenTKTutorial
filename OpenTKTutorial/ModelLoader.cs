using Assimp;
using System.Linq;
using System.Collections.Generic;

namespace OpenTKTutorial
{
    public static class ModelLoader
    {
        public static Model LoadModel(string path)
        {
            var materialFactory = new MaterialFactory();

            using (var importer = new AssimpContext())
            {
                var scene = importer.ImportFileFromStream(Utility.GetEmbeddedResourceStream(path),
                    PostProcessSteps.Triangulate |
                    PostProcessSteps.GenerateNormals |
                    PostProcessSteps.FlipUVs
                );

                Utility.Assert(scene.HasMeshes, "Scene should have at least one material.");
                Utility.Assert(scene.HasMaterials, "Scene should have at least one material.");

                var meshes = new Mesh[scene.MeshCount];
                var materialIndices = new int[scene.MeshCount];
                for (var meshIndex = 0; meshIndex < meshes.Length; ++meshIndex)
                {
                    var mesh = scene.Meshes[meshIndex];

                    Utility.Assert(mesh.HasFaces, "Scene should have any face.");
                    Utility.Assert(mesh.HasNormals, "Scene should have any normal.");
                    Utility.Assert(mesh.HasFaces, "Scene should have any face.");

                    var meshDescriptor = new Mesh.Descriptor();

                    meshDescriptor.Name = mesh.Name;
                    meshDescriptor.Positions = new float[mesh.VertexCount * 3];
                    for (var vertexIndex = 0; vertexIndex < mesh.Vertices.Count; ++vertexIndex)
                    {
                        var vertex = mesh.Vertices[vertexIndex];
                        meshDescriptor.Positions[3 * vertexIndex + 0] = vertex.X * 0.01f;
                        meshDescriptor.Positions[3 * vertexIndex + 1] = vertex.Y * 0.01f;
                        meshDescriptor.Positions[3 * vertexIndex + 2] = vertex.Z * 0.01f;
                    }

                    meshDescriptor.Normals = new float[mesh.Normals.Count * 3];
                    for (var normalIndex = 0; normalIndex < mesh.Normals.Count; ++normalIndex)
                    {
                        var normal = mesh.Normals[normalIndex];
                        meshDescriptor.Normals[3 * normalIndex + 0] = normal.X;
                        meshDescriptor.Normals[3 * normalIndex + 1] = normal.Y;
                        meshDescriptor.Normals[3 * normalIndex + 2] = normal.Z;
                    }

                    for (var textureCoordsChannel = 0; textureCoordsChannel < mesh.TextureCoordinateChannelCount; ++textureCoordsChannel)
                    {
                        if (mesh.HasTextureCoords(textureCoordsChannel))
                        {
                            var originalTextureCoordinates = mesh.TextureCoordinateChannels[textureCoordsChannel];
                            var textureCoordinates = new float[originalTextureCoordinates.Count * 2];
                            for (var coordinateIndex = 0; coordinateIndex < originalTextureCoordinates.Count; ++coordinateIndex)
                            {
                                var textureCoordinate = originalTextureCoordinates[coordinateIndex];
                                textureCoordinates[2 * coordinateIndex + 0] = textureCoordinate.X;
                                textureCoordinates[2 * coordinateIndex + 1] = textureCoordinate.Y;
                            }

                            switch (textureCoordsChannel)
                            {
                                case 0:
                                    meshDescriptor.TextureCoordinates1 = textureCoordinates;
                                    break;
                                case 1:
                                    meshDescriptor.TextureCoordinates2 = textureCoordinates;
                                    break;
                                case 2:
                                    meshDescriptor.TextureCoordinates3 = textureCoordinates;
                                    break;
                                case 3:
                                    meshDescriptor.TextureCoordinates4 = textureCoordinates;
                                    break;
                            }
                        }
                    }

                    meshDescriptor.Indices = mesh.GetUnsignedIndices();

                    meshes[meshIndex] = new Mesh(meshDescriptor);
                    materialIndices[meshIndex] = mesh.MaterialIndex;
                }

                var baseDirectory = System.IO.Path.GetDirectoryName(path);

                var materials = scene.Materials
                    .Select(material => materialFactory.CreateMaterial(material, baseDirectory))
                    .ToArray();

                return new Model(new Model.Descriptor()
                {
                    Meshes = meshes,
                    MaterialIndices = materialIndices,
                    Materials = materials,
                });
            }
        }
    }
}