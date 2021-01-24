using Assimp;
using System.Linq;

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
                    PostProcessSteps.GenerateNormals |
                    PostProcessSteps.CalculateTangentSpace
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

                    var meshDescriptor = new Mesh.Descriptor();

                    meshDescriptor.Positions = new float[mesh.VertexCount * 3];
                    for (var vertexIndex = 0; vertexIndex < mesh.Vertices.Count; ++vertexIndex)
                    {
                        var vertex = mesh.Vertices[vertexIndex];
                        meshDescriptor.Positions[3 * vertexIndex + 0] = vertex.X;
                        meshDescriptor.Positions[3 * vertexIndex + 1] = vertex.Y;
                        meshDescriptor.Positions[3 * vertexIndex + 2] = vertex.Z;
                    }

                    meshDescriptor.Normals = new float[mesh.Normals.Count * 3];
                    for (var normalIndex = 0; normalIndex < mesh.Normals.Count; ++normalIndex)
                    {
                        var normal = mesh.Normals[normalIndex];
                        meshDescriptor.Normals[3 * normalIndex + 0] = normal.X;
                        meshDescriptor.Normals[3 * normalIndex + 1] = normal.Y;
                        meshDescriptor.Normals[3 * normalIndex + 2] = normal.Z;
                    }

                    meshDescriptor.Indices = mesh.GetUnsignedIndices();

                    meshes[meshIndex] = new Mesh(meshDescriptor);
                    materialIndices[meshIndex] = mesh.MaterialIndex;
                }

                var materials = scene.Materials
                    .Select(material => materialFactory.CreateMaterial(material))
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