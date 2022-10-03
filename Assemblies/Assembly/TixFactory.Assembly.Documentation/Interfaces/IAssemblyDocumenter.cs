using System;
using System.Collections.Generic;
using System.Reflection;

namespace TixFactory.Assembly.Documentation
{
    /// <summary>
    /// Builds <see cref="IAssemblyDocumentation"/>s.
    /// </summary>
    public interface IAssemblyDocumenter
    {
        /// <summary>
        /// The names of the assemblies that have been initialized to document.
        /// </summary>
        ISet<string> Names { get; }

        /// <summary>
        /// Gets an <see cref="IAssemblyDocumentation"/> by specified <see cref="Type"/>.
        /// </summary>
        /// <remarks>
        /// The type must be present in one of the assemblies the class has been initialized with.
        /// </remarks>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <returns>The <see cref="IAssemblyDocumentation"/>.</returns>
        IAssemblyDocumentation GetTypeDocumentation(Type type);

        /// <summary>
        /// Gets an <see cref="IAssemblyDocumentation"/> by specified <see cref="FieldInfo"/>.
        /// </summary>
        /// <remarks>
        /// The field must be present in one of the assemblies the class has been initialized with.
        /// </remarks>
        /// <param name="fieldInfo">The <see cref="FieldInfo"/>.</param>
        /// <returns>The <see cref="IAssemblyDocumentation"/>.</returns>
        IAssemblyDocumentation GetFieldDocumentation(FieldInfo fieldInfo);

        /// <summary>
        /// Gets an <see cref="IAssemblyDocumentation"/> by specified <see cref="PropertyInfo"/>.
        /// </summary>
        /// <remarks>
        /// The property must be present in one of the assemblies the class has been initialized with.
        /// </remarks>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        /// <returns>The <see cref="IAssemblyDocumentation"/>.</returns>
        IAssemblyDocumentation GetPropertyDocumentation(PropertyInfo propertyInfo);
    }
}
