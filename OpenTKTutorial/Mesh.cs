using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace OpenTKTutorial
{
    public class Mesh : System.IDisposable
    {
        public string Name { get; } = null;
        public float[] Positions { get; } = null;
        public float[] Normals { get; } = null;
        public float[] Colors { get; } = null;
        public uint [] Indices { get; } = null;

        public float[] TextureCoordinates1 { get; } = null;
        public float[] TextureCoordinates2 { get; } = null;
        public float[] TextureCoordinates3 { get; } = null;
        public float[] TextureCoordinates4 { get; } = null;

        public OpenGLBuffer VertexBuffer { get; }
        public OpenGLBuffer IndexBuffer { get; }
        public OpenGLVertexArray VertexArray { get; }
        private VertexAttribute[] VertexAttributes { get; }

        public Mesh(Descriptor descriptor)
        {
            Utility.Assert(descriptor.Positions != null && descriptor.Positions.Length > 0, "Vertex position should not be empty.");
            Utility.Assert(descriptor.Normals != null && descriptor.Normals.Length > 0, "Vertex position should not be empty.");
            Utility.Assert(descriptor.Indices != null && descriptor.Indices.Length > 0, "Vertex position should not be empty.");

            VertexAttributes = CalculateAttributes(descriptor);

            Name = descriptor.Name;
            VertexBuffer = new OpenGLBuffer(VertexAttributes.Length, BufferTarget.ArrayBuffer);
            IndexBuffer = new OpenGLBuffer(1, BufferTarget.ElementArrayBuffer);
            VertexArray = new OpenGLVertexArray(1);

            Indices = new uint[descriptor.Indices.Length];
            System.Buffer.BlockCopy(descriptor.Indices, 0, Indices, 0, Indices.Length * sizeof(uint));
            IndexBuffer.Bind(0);
            IndexBuffer.SetData(0, sizeof(uint) * Indices.Length, Indices, BufferUsageHint.StaticDraw);

            VertexArray.Bind(0);

            foreach (var vertexAttribute in VertexAttributes)
            {
                VertexBuffer.Bind(vertexAttribute.Index);
                switch (vertexAttribute.Type)
                {
                    case VertexAttributeType.Position:
                        Positions = new float[descriptor.Positions.Length];
                        System.Buffer.BlockCopy(descriptor.Positions, 0, Positions, 0, Positions.Length * sizeof(float));
                        VertexBuffer.SetData(vertexAttribute.Index, vertexAttribute.ElementSizeInByte * Positions.Length, Positions, BufferUsageHint.StaticDraw);
                        break;
                    case VertexAttributeType.Normal:
                        Normals = new float[descriptor.Normals.Length];
                        System.Buffer.BlockCopy(descriptor.Normals, 0, Normals, 0, Normals.Length * sizeof(float));
                        VertexBuffer.SetData(vertexAttribute.Index, vertexAttribute.ElementSizeInByte * Normals.Length, Normals, BufferUsageHint.StaticDraw);
                        break;
                    case VertexAttributeType.Color:
                        Colors = new float[descriptor.Colors.Length];
                        System.Buffer.BlockCopy(descriptor.Colors, 0, Colors, 0, Colors.Length * sizeof(float));
                        VertexBuffer.SetData(vertexAttribute.Index, vertexAttribute.ElementSizeInByte * Colors.Length, Colors, BufferUsageHint.StaticDraw);
                        break;
                    case VertexAttributeType.UV1:
                        TextureCoordinates1 = new float[descriptor.TextureCoordinates1.Length];
                        System.Buffer.BlockCopy(descriptor.TextureCoordinates1, 0, TextureCoordinates1, 0, TextureCoordinates1.Length * sizeof(float));
                        VertexBuffer.SetData(vertexAttribute.Index, vertexAttribute.ElementSizeInByte * TextureCoordinates1.Length, TextureCoordinates1, BufferUsageHint.StaticDraw);
                        break;
                    case VertexAttributeType.UV2:
                        TextureCoordinates2 = new float[descriptor.TextureCoordinates2.Length];
                        System.Buffer.BlockCopy(descriptor.TextureCoordinates2, 0, TextureCoordinates2, 0, TextureCoordinates2.Length * sizeof(float));
                        VertexBuffer.SetData(vertexAttribute.Index, vertexAttribute.ElementSizeInByte * TextureCoordinates2.Length, TextureCoordinates2, BufferUsageHint.StaticDraw);
                        break;
                    case VertexAttributeType.UV3:
                        TextureCoordinates3 = new float[descriptor.TextureCoordinates3.Length];
                        System.Buffer.BlockCopy(descriptor.TextureCoordinates3, 0, TextureCoordinates3, 0, TextureCoordinates3.Length * sizeof(float));
                        VertexBuffer.SetData(vertexAttribute.Index, vertexAttribute.ElementSizeInByte * TextureCoordinates3.Length, TextureCoordinates3, BufferUsageHint.StaticDraw);
                        break;
                    case VertexAttributeType.UV4:
                        TextureCoordinates4 = new float[descriptor.TextureCoordinates4.Length];
                        System.Buffer.BlockCopy(descriptor.TextureCoordinates4, 0, TextureCoordinates4, 0, TextureCoordinates4.Length * sizeof(float));
                        VertexBuffer.SetData(vertexAttribute.Index, vertexAttribute.ElementSizeInByte * TextureCoordinates4.Length, TextureCoordinates4, BufferUsageHint.StaticDraw);
                        break;
                }

                VertexArray.EnableAttribute(0, vertexAttribute.Index, vertexAttribute.ElementCount, vertexAttribute.PointerType, false, vertexAttribute.SizeInByte, 0);
            }

            VertexArray.Unbind();
        }

        public void Dispose()
        {
        }

        public void Bind()
        {
            VertexArray.Bind(0);
            IndexBuffer.Bind(0);
        }

        public void Unbind()
        {
            VertexArray.Unbind();
        }

        private VertexAttribute[] CalculateAttributes(Descriptor descriptor)
        {
            var attributes = new List<VertexAttribute>();

            attributes.Add(new VertexAttribute(0, VertexAttributeType.Position));
            attributes.Add(new VertexAttribute(1, VertexAttributeType.Normal));

            if (descriptor.TextureCoordinates1 != null && descriptor.TextureCoordinates1.Length > 0)
            {
                attributes.Add(new VertexAttribute(attributes.Count, VertexAttributeType.UV1));
            }
            if (descriptor.TextureCoordinates2 != null && descriptor.TextureCoordinates2.Length > 0)
            {
                attributes.Add(new VertexAttribute(attributes.Count, VertexAttributeType.UV2));
            }
            if (descriptor.TextureCoordinates3 != null && descriptor.TextureCoordinates3.Length > 0)
            {
                attributes.Add(new VertexAttribute(attributes.Count, VertexAttributeType.UV3));
            }
            if (descriptor.TextureCoordinates4 != null && descriptor.TextureCoordinates4.Length > 0)
            {
                attributes.Add(new VertexAttribute(attributes.Count, VertexAttributeType.UV4));
            }

            return attributes.ToArray();
        }

        public class Descriptor
        {
            public string Name;
            public float[] Positions;
            public float[] Normals;
            public float[] Colors;
            public uint[] Indices;
            public float[] TextureCoordinates1;
            public float[] TextureCoordinates2;
            public float[] TextureCoordinates3;
            public float[] TextureCoordinates4;
        }

        public struct VertexAttribute
        {
            public int Index { get; }
            public VertexAttributeType Type { get; }
            public int ElementSizeInByte
            {
                get
                {
                    switch (Type)
                    {
                        case VertexAttributeType.Position:
                        case VertexAttributeType.Normal:
                            return sizeof(float);
                        case VertexAttributeType.Color:
                            return sizeof(float);
                        case VertexAttributeType.UV1:
                        case VertexAttributeType.UV2:
                        case VertexAttributeType.UV3:
                        case VertexAttributeType.UV4:
                            return sizeof(float);
                    }

                    throw new OpenTKTutorialException("Invalid type.");
                }
            }

            public int SizeInByte
            {
                get
                {
                    switch (Type)
                    {
                        case VertexAttributeType.Position:
                        case VertexAttributeType.Normal:
                            return 3 * sizeof(float);
                        case VertexAttributeType.Color:
                            return 4 * sizeof(float);
                        case VertexAttributeType.UV1:
                        case VertexAttributeType.UV2:
                        case VertexAttributeType.UV3:
                        case VertexAttributeType.UV4:
                            return 2 * sizeof(float);
                    }

                    throw new OpenTKTutorialException("Invalid type.");
                }
            }

            public int ElementCount
            {
                get
                {
                    switch (Type)
                    {
                        case VertexAttributeType.Position:
                        case VertexAttributeType.Normal:
                            return 3;
                        case VertexAttributeType.Color:
                            return 4;
                        case VertexAttributeType.UV1:
                        case VertexAttributeType.UV2:
                        case VertexAttributeType.UV3:
                        case VertexAttributeType.UV4:
                            return 2;
                    }

                    throw new OpenTKTutorialException("Invalid type.");
                }
            }

            public VertexAttribPointerType PointerType
            {
                get
                {
                    switch (Type)
                    {
                        case VertexAttributeType.Position:
                        case VertexAttributeType.Normal:
                            return VertexAttribPointerType.Float;
                        case VertexAttributeType.Color:
                            return VertexAttribPointerType.Float;
                        case VertexAttributeType.UV1:
                        case VertexAttributeType.UV2:
                        case VertexAttributeType.UV3:
                        case VertexAttributeType.UV4:
                            return VertexAttribPointerType.Float;
                    }

                    throw new OpenTKTutorialException("Invalid type.");
                }
            }

            public VertexAttribute(int index, VertexAttributeType type)
            {
                Index = index;
                Type = type;
            }
        }

        public enum VertexAttributeType
        {
            Position,
            Color,
            Normal,
            UV1,
            UV2,
            UV3,
            UV4,
        }
    }
}