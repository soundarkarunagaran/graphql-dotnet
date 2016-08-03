using GraphQL.Language;
using GraphQL.Language.AST;

namespace GraphQL.Execution
{
    public interface IDocumentBuilder
    {
        Document Build(string data);
    }
}
