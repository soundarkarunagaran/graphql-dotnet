using System.Collections.Generic;
using System.Linq;
using GraphQL.Language;
using GraphQL.Language.AST;
using GraphQL.Types;
using GraphQL.Validation.Rules;

namespace GraphQL.Validation
{
    public interface IDocumentValidator
    {
        IValidationResult Validate(
            string originalQuery,
            ISchema schema,
            Document document,
            IEnumerable<IValidationRule> rules = null);
    }

    public class DocumentValidator : IDocumentValidator
    {
        public IValidationResult Validate(
            string originalQuery,
            ISchema schema,
            Document document,
            IEnumerable<IValidationRule> rules = null)
        {
            var context = new ValidationContext
            {
                OriginalQuery = originalQuery,
                Schema = schema,
                Document = document,
                TypeInfo = new TypeInfo(schema)
            };

            if (rules == null)
            {
                rules = CoreRules();
            }

            var visitors = rules.Select(x => x.Validate(context)).ToList();

            visitors.Insert(0, context.TypeInfo);
#if DEBUG
            visitors.Insert(1, new DebugNodeVisitor());
#endif

            var basic = new BasicVisitor(visitors.ToArray());

            basic.Visit(document);

            var result = new ValidationResult();
            result.Errors.AddRange(context.Errors);
            return result;
        }

        public static List<IValidationRule> CoreRules()
        {
            var rules = new List<IValidationRule>
            {
                new UniqueOperationNames(),
                new LoneAnonymousOperation(),
                new KnownTypeNames(),
                new VariablesAreInputTypes(),
                new ScalarLeafs(),
                new NoUndefinedVariables(),
                new KnownArgumentNames(),
                new ArgumentsOfCorrectType(),
                new DefaultValuesOfCorrectType(),
                new VariablesInAllowedPosition(),
                new UniqueInputFieldNames()
            };
            return rules;
        }
    }
}
