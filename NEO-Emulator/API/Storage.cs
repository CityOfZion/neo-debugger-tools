using LunarParser;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace Neo.Emulator.API
{
    public class Storage : IInteropInterface
    {
        public static int lastStorageLength; // this is an hack for now...

        public Dictionary<byte[], byte[]> entries = new Dictionary<byte[], byte[]>(new ByteArrayComparer());
        public int sizeInBytes { get; private set; }

        public void Write(byte[] key, byte[] data)
        {
            if (entries.ContainsKey(key))
            {
                var oldEntry = entries[key];
                if (oldEntry != null)
                {
                    sizeInBytes -= oldEntry.Length;
                }
            }

            entries[key] = data;

            if (data != null)
            {
                sizeInBytes += data.Length;
            }

            lastStorageLength = data != null ? data.Length : 0;
        }

        public byte[] Read(byte[] key)
        {
            byte[] data = null;
            if (entries.ContainsKey(key))
            {
                data = entries[key];
            }

            if (data == null)
            {
                data = new byte[0];
            }

            return data;
        }

        public void Remove(byte[] key)
        {
            if (entries.ContainsKey(key))
            {
                var oldEntry = entries[key];
                if (oldEntry != null)
                {
                    sizeInBytes -= oldEntry.Length;
                }

                entries.Remove(key);
            }
        }

        internal bool Load(DataNode root)
        {
            sizeInBytes = 0;
            entries.Clear();

            foreach (var child in root.Children)
            {
                if (child.Name == "entry")
                {
                    var key = Convert.FromBase64String(child.GetString("key"));
                    var data = Convert.FromBase64String(child.GetString("data"));

                    sizeInBytes += data.Length;

                    entries[key] = data;
                }

            }

            return true;
        }

        public DataNode Save()
        {
            var result = DataNode.CreateObject("storage");

            foreach (var entry in entries)
            {
                var key = Convert.ToBase64String(entry.Key);
                var data = Convert.ToBase64String(entry.Value);

                var child = DataNode.CreateObject("entry");
                child.AddField("key", key);
                child.AddField("data", data);

                result.AddNode(child);
            }

            return result;
        }

        #region SMART CONTRACT API

        [Syscall("Neo.Storage.GetContext")]
        public static bool GetCurrentContext(ExecutionEngine engine)
        {
            var storage = engine.GetStorage();

            var context = new Neo.VM.Types.InteropInterface(storage);
            engine.EvaluationStack.Push(context);

            //returns StorageContext 
            return true;
        }

        [Syscall("Neo.Storage.Get", 0.1)]
        public static bool Get(ExecutionEngine engine)
        {
            //StorageContext context, byte[] key
            //OR
            //StorageContext context, string key

            //returns byte[]

            var obj = engine.EvaluationStack.Pop();
            var item = engine.EvaluationStack.Pop();
            
            var key = item.GetByteArray();

            var storage = ((VM.Types.InteropInterface)obj).GetInterface<Storage>();
            var data = storage.Read(key);

            var result = new VM.Types.ByteArray(data);
            engine.EvaluationStack.Push(result);

            return true;
        }

        [Syscall("Neo.Storage.Put", 1)]
        public static bool Put(ExecutionEngine engine)
        {
            //StorageContext context, byte[] key, byte[] value
            //OR
            //StorageContext context, byte[] key, BigInteger value
            //OR
            //StorageContext context, byte[] key, string value
            //OR
            //StorageContext context, string key, byte[] value
            //OR
            //StorageContext context, string key, BigInteger value
            //OR
            //StorageContext context, string key, string value
            // return void

            var obj = engine.EvaluationStack.Pop();
            var keyItem = engine.EvaluationStack.Pop();
            var dataItem = engine.EvaluationStack.Pop();

            var key = keyItem.GetByteArray();
            var data = dataItem.GetByteArray();

            var storage = ((VM.Types.InteropInterface)obj).GetInterface<Storage>();
            storage.Write(key, data);

            return true;
        }

        [Syscall("Neo.Storage.Delete")]
        public static bool Delete(ExecutionEngine engine)
        {
            //StorageContext context, byte[] key
            //OR
            //StorageContext context, string key

            var context = engine.EvaluationStack.Pop();
            var keyItem = engine.EvaluationStack.Pop();

            var key = keyItem.GetByteArray();

            var storage = engine.GetStorage();
            storage.Remove(key);

            return true;
        }

        #endregion
    }
}
