using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GraphQL;
using GraphQL.Types;
using GraphQL.SystemTextJson;

public class Droid
{
  public string Id { get; set; }
  public string Name { get; set; }
}

public static class DroidData
{
  private static List<Droid>
  _droids = new List<Droid>
  {
    new Droid { Id = "123", Name = "R2-D2" },
    new Droid { Id = "234", Name = "C3PO" }
  };

  public static IEnumerable<Droid> GetAll() => _droids;
  
  public static Droid GetById(string id) => _droids.FirstOrDefault(droid => droid.Id == id);

  public static void Add(Droid droid) => _droids.Add(droid);
}

public class Query
{
  [GraphQLMetadata("droids")]
  public IEnumerable<Droid> GetAllDroids() => DroidData.GetAll();
  
  [GraphQLMetadata("droid")]
  public Droid GetDroid(string id) => DroidData.GetById(id);
}

public class Mutation
{
  [GraphQLMetadata("addDroid")]
  public Droid AddDroid(string name)
  {
    var droid = new Droid
    {
      Id = $"{Guid.NewGuid()}",
      Name = name
    };

    DroidData.Add(droid);

    return droid;
  }
}

static partial class Program {
  static async Task Main(string[] args) {
    var schema = Schema.For(@"
      type Droid {
        id: ID
        name: String
      }
    
      type Query {
        droids: [Droid]
        droid(id: ID): Droid
      }

      type Mutation {
        addDroid(name: String): Droid
        updateDroid(id: String, name: String): Droid
        deleteDroid(id: String): Droid
      }
    ", _ => {
        _.Types.Include<Query>();
      _.Types.Include<Mutation>();
    });

    var id = args.Length > 0 ? args[0] : "123";
    Console.WriteLine($"Querying Droid with ID: {id}");
    var json = await schema.ExecuteAsync(_ =>
    {
      _.Query = $"{{ droid(id: \"{id}\") {{ id name }} }}";
    });
    
    Console.WriteLine(json);

    Console.WriteLine("Adding Droid: Gr0nk");
    json = await schema.ExecuteAsync(_ =>
    {
      _.Query = "mutation { addDroid(name: \"Gr0nk\") { id name } }";
    });
    
    Console.WriteLine(json);

    Console.WriteLine("Querying all Droids");
    json = await schema.ExecuteAsync(_ =>
    {
      _.Query = "{ droids { id name } }";
    });
    
    Console.WriteLine(json);
  }
}