using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GigyaManagement.Core.Services.LockService
{
    public class Lock
    {
        [JsonPropertyName("locks")]
        public Dictionary<string, string> LockList { get; set; } = new();
    }

    public interface ILockService
    {
        Dictionary<string, string> GetLocksForFolder(string folder);
        Dictionary<string, string> WriteLock(string folder);
        IEnumerable<(string op, string file)> Validate(string folder);
        public void LockRecursive(string folder);
    }
    public class LockService : ILockService
    {
        const string lockFileName = "lock.json";

        public void LockRecursive(string folder)
        {
            if (!Directory.Exists(folder))
            {
                throw new Exception("Locking is provider only for existing folders");
            }

            var stack = new Stack<string>();
            stack.Push(folder);

            while(stack.Count > 0)
            {
                var cur = stack.Pop();
                var next = Directory.GetDirectories(cur);
                
                foreach(var n in next)
                    stack.Push(n);
                
                WriteLock(cur);
            }
            
        }

        public Dictionary<string, string> GetLocksForFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                throw new Exception("Locking is provider only for existing folders");
            }

            return Directory
                .GetFiles(folder)
                .Where(x => Path.GetFileName(x) != lockFileName)
                .Select(x => (file: x, content: File.ReadAllBytes(x)))
                .Select(x => (x.file, hash: SHA1.HashData(x.content)))
                .ToDictionary(x => Path.GetRelativePath(folder,x.file), x => Convert.ToBase64String(x.hash));
        }

        public IEnumerable<(string op, string file)> Validate(string folder)
        {
            var filePath = Path.Combine(folder, lockFileName);
         
            if (File.Exists(filePath)) return Enumerable.Empty<(string, string)>();

            var currentJson = File.ReadAllText(filePath);
            var fileLock = JsonSerializer.Deserialize<Lock>(currentJson);

            if (fileLock == null) return Enumerable.Empty<(string,string)>();

            var currentLock = GetLocksForFolder(folder);
            var locks = fileLock.LockList;

            var list = new List<(string op, string file)>();
            
            var added = currentLock.Keys.Except(locks.Keys).Select(x => ("added", x));
            list.AddRange(added);

            currentLock.Keys
                .Intersect(locks.Keys)
                .Where(x => currentLock[x] != locks[x])
                .ToList()
                .ForEach(x =>
                {
                    list.Add(("changed", x));
                });
            
            return list;
        }

        public Dictionary<string, string> WriteLock(string folder)
        {
            var locks = GetLocksForFolder(folder);
            if (locks.Count == 0) return null;

            var filePath = Path.Combine(folder, lockFileName);

            if (File.Exists(filePath))
            {
                File.Move(filePath, Path.Combine(folder, "lock.prev.json"));
            }

            File.WriteAllText(Path.Combine(folder, filePath), JsonSerializer.Serialize(locks, GlobalUsings.JsonSerializerOptions));

            return locks;
        }
    }
}
