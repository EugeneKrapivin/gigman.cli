using Serilog;

using System.Text.Json;

namespace GigyaManagement.CLI.Services.Context;

public class ContextService : IContextService
{
    private readonly ContextModel _context;
    private readonly string _contextFile;

    const string _configFile = "gigman.json";

    public ContextService()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        _contextFile = Path.Combine(folder, ".gigman_workspaces", _configFile);
        if (!File.Exists(_contextFile))
        {
            var file = File.Create(_contextFile);
            file.Close();
            _context = new();
            Persist();
        }

        _context = Load();
    }

    private void Persist(FileStream? stream = null)
    {
        if (stream is null)
        {
            using var file = File.OpenWrite(_contextFile);
            JsonSerializer.Serialize(new Utf8JsonWriter(file), _context, typeof(ContextModel), new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        }
        else
        {
            JsonSerializer.Serialize(new Utf8JsonWriter(stream), _context, typeof(ContextModel), new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        }
    }

    private ContextModel Load()
    {
        if (!File.Exists(_contextFile))
        {
            return new ContextModel();
        }

        using var file = File.OpenRead(_contextFile);

        if (file.Length > 0)
        {
            var ctx = JsonSerializer.Deserialize<ContextModel>(new StreamReader(file).BaseStream);

            if (ctx == null) throw new Exception("Failed to parse context");

            return ctx;
        }
        else
        {
            return new ContextModel();
        }
    }

    public ContextModel GetAllContexts()
    {
        return _context;
    }

    public WorkspaceContext? GetCurrentContext()
    {
        if (_context.DefinedContexts.Count == 0 && string.IsNullOrEmpty(_context.CurrentContext)) // fresh
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(_context.CurrentContext))
        {
            return null;
        }
        var context = _context!.DefinedContexts.SingleOrDefault(x => x.Name == _context.CurrentContext);
        
        if (context == null)
        {
            Log.Logger.Error("The defined context is not found, please use list and then set to select a new context");
            return null;
        }
        
        return context;
    }

    public WorkspaceContext CreateNewContext(WorkspaceContext context)
    {
        if (_context!.DefinedContexts.Any(x => x.Name == context.Name))
        {
            throw new Exception($"A context with the name \"{context.Name}\" already exists");
        }

        _context.DefinedContexts.Add(context);

        Persist();

        return context;
    }

    public WorkspaceContext SetContext(string context)
    {
        if (_context.CurrentContext == context)
        {
            return _context.DefinedContexts.Single(x => x.Name == context);
        }

        if (!_context!.DefinedContexts.Any(x => x.Name == context))
        {
            throw new Exception($"context with name \"{context}\" is not defined");
        }

        _context.CurrentContext = context;
        Persist();

        return _context.DefinedContexts.Single(x => x.Name == context);
    }
}
