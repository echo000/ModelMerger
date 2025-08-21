using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PhilLibX
{
    /// <summary>
    /// Cast Node Identifier
    /// </summary>
    public enum CastNodeID : uint
    {
        /// <summary>
        /// Root node
        /// </summary>
        Root = 0x746F6F72,
        /// <summary>
        /// Model node
        /// </summary>
        Model = 0x6C646F6D,
        /// <summary>
        /// Mesh node
        /// </summary>
        Mesh = 0x6873656D,
        /// <summary>
        /// Blendshape node
        /// </summary>
        BlendShape = 0x68736C62,
        /// <summary>
        /// Skeleton node
        /// </summary>
        Skeleton = 0x6C656B73,
        /// <summary>
        /// Bone node
        /// </summary>
        Bone = 0x656E6F62,
        /// <summary>
        /// IKHandle node
        /// </summary>
        IKHandle = 0x64686B69,
        /// <summary>
        /// Constraint node
        /// </summary>
        Constraint = 0x74736E63,
        /// <summary>
        /// Animation node
        /// </summary>
        Animation = 0x6D696E61,
        /// <summary>
        /// Curve node
        /// </summary>
        Curve = 0x76727563,
        /// <summary>
        /// CurveModeOverride node
        /// </summary>
        CurveModeOverride = 0x564F4D43,
        /// <summary>
        /// NotificationTrack node
        /// </summary>
        NotificationTrack = 0x6669746E,
        /// <summary>
        /// Material node
        /// </summary>
        Material = 0x6C74616D,
        /// <summary>
        /// File node
        /// </summary>
        File = 0x656C6966,
        /// <summary>
        /// Instance node
        /// </summary>
        Instance = 0x74736E69,
    }

    /// <summary>
    /// Cast Property Identifier
    /// </summary>
    public enum CastPropertyId : ushort
    {
        /// <summary>
        /// Byte property
        /// </summary>
        Byte = 'b',
        /// <summary>
        /// Short property
        /// </summary>
        Short = 'h',
        /// <summary>
        /// Integer32 property
        /// </summary>
        Integer32 = 'i',
        /// <summary>
        /// Integer64 property
        /// </summary>
        Integer64 = 'l',
        /// <summary>
        /// Float property
        /// </summary>
        Float = 'f',
        /// <summary>
        /// Double property
        /// </summary>
        Double = 'd',
        /// <summary>
        /// String property
        /// </summary>
        String = 's',
        /// <summary>
        /// Vector2 property
        /// </summary>
        Vector2 = 'v' << 8 | '2',
        /// <summary>
        /// Vector3 property
        /// </summary>
        Vector3 = 'v' << 8 | '3',
        /// <summary>
        /// Vector4 property
        /// </summary>
        Vector4 = 'v' << 8 | '4'
    };

    /// <summary>
    /// Cast Property
    /// </summary>
    public class CastProperty
    {
        /// <summary>
        /// Identifier of the property
        /// </summary>
        public CastPropertyId Identifier;
        /// <summary>
        /// Number of elements in the property
        /// </summary>
        public ulong Elements;
        /// <summary>
        /// Property buffer
        /// </summary>
        public List<byte> Buffer;

        /// <summary>
        /// Create a new CastProperty
        /// </summary>
        public CastProperty()
        {
            Identifier = CastPropertyId.Byte;
            Elements = 0;
            Buffer = new List<byte>();
        }

        /// <summary>
        /// Writes the string to the property buffer
        /// </summary>
        /// <param name="data">Data input</param>
        public void Write(string data)
        {
            Elements++;
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            Buffer.AddRange(bytes);
            Buffer.Add(0);
        }

        /// <summary>
        /// Writes the generic to the buffer
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <param name="data">Data input</param>
        public void Write<T>(T data)
        {
            Elements++;

            // Convert the generic data to bytes
            int size = Marshal.SizeOf(data);
            byte[] bytes = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(data, ptr, false);
                Marshal.Copy(ptr, bytes, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            Buffer.AddRange(bytes);
        }

        /// <summary>
        /// Writes the byte array to the buffer
        /// </summary>
        /// <param name="data">Data input</param>
        public void Write(byte[] data)
        {
            Elements++;
            Buffer.AddRange(data);
        }
    }

    /// <summary>
    /// Cast Node
    /// </summary>
    public class CastNode
    {
        /// <summary>
        /// Identifier of the node
        /// </summary>
        public CastNodeID Identifier;
        /// <summary>
        /// Node hash
        /// </summary>
        public ulong Hash;
        /// <summary>
        /// Node properties
        /// </summary>
        public SortedDictionary<string, CastProperty> Properties;
        /// <summary>
        /// Node's Children
        /// </summary>
        public List<CastNode> Children;

        /// <summary>
        /// Creates a new cast node with default values
        /// </summary>
        public CastNode()
        {
            Identifier = CastNodeID.Root;
            Hash = 0;
            Children = new List<CastNode>();
            Properties = new SortedDictionary<string, CastProperty>();
        }

        /// <summary>
        /// Creates a new cast node with the specified identifier
        /// </summary>
        /// <param name="id"></param>
        public CastNode(CastNodeID id)
        {
            Identifier = id;
            Hash = 0;
            Children = new List<CastNode>();
            Properties = new SortedDictionary<string, CastProperty>();
        }

        /// <summary>
        /// Creates a new castnode with the specified identifier and hash
        /// </summary>
        /// <param name="id">Cast node Idenfitier</param>
        /// <param name="hash">Hash</param>
        public CastNode(CastNodeID id, ulong hash)
        {
            Identifier = id;
            Hash = hash;
            Children = new List<CastNode>();
            Properties = new SortedDictionary<string, CastProperty>();
        }

        /// <summary>
        /// Adds a property to a cast node
        /// </summary>
        /// <param name="propName">The name of the property</param>
        /// <param name="id">The ID of the property</param>
        /// <returns></returns>
        public CastProperty AddProperty(string propName, CastPropertyId id)
        {
            var prop = new CastProperty
            {
                Identifier = id
            };
            Properties.Add(propName, prop);
            return prop;
        }

        /// <summary>
        /// Adds a property to a cast node with a specified capacity
        /// </summary>
        /// <param name="propName">The name of the property</param>
        /// <param name="id">The ID of the property</param>
        /// <param name="capacity">The capacity</param>
        /// <returns></returns>
        public CastProperty AddProperty(string propName, CastPropertyId id, int capacity)
        {
            var prop = new CastProperty
            {
                Identifier = id,
                Buffer = new List<byte>(capacity)
            };
            Properties.Add(propName, prop);
            return prop;
        }

        /// <summary>
        /// Sets the property of a cast node
        /// </summary>
        /// <param name="propName">name of the property</param>
        /// <param name="value">String input</param>
        public void SetProperty(string propName, string value)
        {
            var prop = new CastProperty
            {
                Identifier = CastPropertyId.String
            };
            prop.Write(value);
            Properties[propName] = prop;
        }

        /// <summary>
        /// Sets the property of a cast node
        /// </summary>
        /// <param name="propName">name of the property</param>
        /// <param name="id">ID of the property</param>
        /// <param name="value">Generic input</param>
        public void SetProperty<T>(string propName, CastPropertyId id, T value)
        {
            var prop = new CastProperty
            {
                Identifier = id
            };
            prop.Write(value);
            Properties[propName] = prop;
        }

        /// <summary>
        /// Gets the size of the cast node
        /// </summary>
        /// <returns>The size of the node in bytes</returns>
        public int Size()
        {
            var result = 24;

            foreach (var prop in Properties)
            {
                result += 8;
                result += prop.Key.Length;
                result += prop.Value.Buffer.Count;
            }

            foreach (var child in Children)
            {
                result += child.Size();
            }

            return result;
        }

        /// <summary>
        /// Adds a child to the cast node
        /// </summary>
        /// <param name="id">The child identifier</param>
        /// <returns>The new child</returns>
        public CastNode AddNode(CastNodeID id)
        {
            Children.Add(new CastNode(id));
            return Children.Last();
        }

        /// <summary>
        /// Adds a child to the cast node with a specified hash
        /// </summary>
        /// <param name="id">The child identifier</param>
        /// <param name="hash">The child hash</param>
        /// <returns>The new child</returns>
        public CastNode AddNode(CastNodeID id, ulong hash)
        {
            Children.Add(new CastNode(id, hash));
            return Children.Last();
        }
    }
}
