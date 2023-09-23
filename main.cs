using System;
using System.Threading.Tasks;
using System.Linq;
using GraphQL;
using GraphQL.Types;
using GraphQL.SystemTextJson; // First add PackageReference to GraphQL.SystemTextJson

public class Droid
{
  public string Id { get; set; }
  public string Name { get; set; }
}

public class Query
{
  private Droid[] _droids = {
    new Droid { Id = "123", Name = "R2-D2" },
    new Droid { Id = "234", Name = "C3PO" }
  };
  
  [GraphQLMetadata("droid")]
  public Droid GetDroid(string id)
  {
    return _droids.FirstOrDefault(droid => droid.Id == id);
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
        droid(id: ID): Droid
      }
    ", _ => {
        _.Types.Include<Query>();
    });

    var id = args.Length > 0 ? args[0] : "123";
    Console.WriteLine(id);
    var json = await schema.ExecuteAsync(_ =>
    {
      _.Query = $"{{ droid(id: \"{id}\") {{ id name }} }}";
    });
    
    Console.WriteLine(json);
  }
}