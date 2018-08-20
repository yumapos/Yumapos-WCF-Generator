namespace WCFGenerator.Common
{
    public struct SrcFile
    {
        public readonly string FileName;
        public readonly string ProjectFolder;
        public readonly string SrcText;

        /// <summary>
        ///     New source code file
        /// </summary>
        /// <param name="fileName">File name with extension</param>
        /// <param name="projectFolder">Folder in project where file can be saved (for example: "Models\Generated" )</param>
        /// <param name="srcText">Text of code</param>
        public SrcFile(string fileName, string projectFolder, string srcText)
        {
            FileName = fileName;
            ProjectFolder = projectFolder;
            SrcText = srcText;
        }
    }
}