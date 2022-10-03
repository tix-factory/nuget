using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TixFactory.Assembly.Documentation
{
    /// <inheritdoc cref="IAssemblyDocumentation"/>
    public class AssemblyDocumentation : IAssemblyDocumentation
    {
        /// <inheritdoc cref="IAssemblyDocumentation.Name"/>
        public string Name { get; set; }

        /// <inheritdoc cref="IAssemblyDocumentation.Summary"/>
        public string Summary { get; set; }

        /// <inheritdoc cref="IAssemblyDocumentation.Remarks"/>
        public string Remarks { get; set; }

        /// <inheritdoc cref="IAssemblyDocumentation.InheritedAssemblyDocumentations"/>
        public ICollection<IAssemblyDocumentation> InheritedAssemblyDocumentations { get; set; }

        public AssemblyDocumentation()
        {
            InheritedAssemblyDocumentations = new List<IAssemblyDocumentation>();
        }

        /// <inheritdoc cref="IAssemblyDocumentation.GetSummary"/>
        public string GetSummary(bool useInheritdoc)
        {
            return GetStringDocumentation(() => Summary, doc => doc.GetSummary(true), useInheritdoc);
        }

        /// <inheritdoc cref="IAssemblyDocumentation.GetRemarks"/>
        public string GetRemarks(bool useInheritdoc)
        {
            return GetStringDocumentation(() => Remarks, doc => doc.GetRemarks(true), useInheritdoc);
        }

        public string BuildComment(int lineIndentionCount, bool useInheritdoc)
        {
            var comment = new StringBuilder();


            var summary = GetSummary(useInheritdoc);
            if (!string.IsNullOrWhiteSpace(summary))
            {
                comment.AppendLine("<summary>");
                comment.AppendLine(summary.Replace("\n", Environment.NewLine));
                comment.AppendLine("</summary>");
            }

            var remarks = GetRemarks(useInheritdoc);
            if (!string.IsNullOrWhiteSpace(remarks))
            {
                comment.AppendLine("<remarks>");
                comment.AppendLine(remarks.Replace("\n", Environment.NewLine));
                comment.AppendLine("</remarks>");
            }

            var builtComment = comment.ToString();
            var indention = new string('\t', lineIndentionCount);
            var lineStart = indention + "/// ";
            var splitter = new[] { Environment.NewLine };

            var commentSplit = builtComment.Split(splitter, StringSplitOptions.None);
            commentSplit = commentSplit.Take(commentSplit.Length - 1).Select(e => lineStart + e.Trim()).ToArray();

            return string.Join(Environment.NewLine, commentSplit);
        }

        private string GetStringDocumentation(Func<string> getContent, Func<IAssemblyDocumentation, string> documentationGetter, bool useInheritdoc)
        {
            var content = getContent();

            if (string.IsNullOrWhiteSpace(content))
            {
                if (useInheritdoc)
                {
                    foreach (var doc in InheritedAssemblyDocumentations)
                    {
                        var inheritedContent = documentationGetter(doc);
                        if (!string.IsNullOrWhiteSpace(inheritedContent))
                        {
                            return inheritedContent;
                        }
                    }
                }

                return null;
            }

            return content;
        }
    }
}
