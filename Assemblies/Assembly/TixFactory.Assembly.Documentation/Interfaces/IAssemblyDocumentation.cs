using System.Collections.Generic;

namespace TixFactory.Assembly.Documentation
{
    /// <summary>
    /// Assembly documentation
    /// </summary>
    public interface IAssemblyDocumentation
    {
        /// <summary>
        /// The XML file name for the documentation.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The summary tag content.
        /// </summary>
        string Summary { get; set; }

        /// <summary>
        /// The remarks tag content.
        /// </summary>
        string Remarks { get; set; }

        /// <summary>
        /// documentation inherited with the inheritdoc tag
        /// </summary>
        ICollection<IAssemblyDocumentation> InheritedAssemblyDocumentations { get; }

        /// <summary>
        /// Gets the summary content.
        /// </summary>
        /// <param name="useInheritdoc">Whether or not to pull from <see cref="InheritedAssemblyDocumentations"/> when summary is not directly specified.</param>
        /// <returns>The summary (or <c>null</c> if no summary can be found.)</returns>
        string GetSummary(bool useInheritdoc);

        /// <summary>
        /// Gets the remarks content.
        /// </summary>
        /// <param name="useInheritdoc">Whether or not to pull from <see cref="InheritedAssemblyDocumentations"/> when remarks is not directly specified.</param>
        /// <returns>The remarks (or <c>null</c> if no remarks can be found.)</returns>
        string GetRemarks(bool useInheritdoc);

        /// <summary>
        /// Builds the XML
        /// </summary>
        /// <remarks>
        /// When <paramref name="useInheritdoc"/> is <c>true</c> no inheritdoc tags will be added to the comment
        /// and all tags could pull from inheritdoc to build final documentation.
        /// When <paramref name="useInheritdoc"/> is <c>false</c> only tags that are directly specified will be added.
        /// </remarks>
        /// <param name="lineIndentionCount">The number of tabs to insert before each line.</param>
        /// <param name="useInheritdoc">Whether or not to pull from <see cref="InheritedAssemblyDocumentations"/> to obtain missing tags.</param>
        /// <returns>The documentation code comment (could be empty.)</returns>
        string BuildComment(int lineIndentionCount, bool useInheritdoc);
    }
}
