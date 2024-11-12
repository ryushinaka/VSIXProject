using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;

using System.Text.RegularExpressions;

namespace VSIXProject1
{
    //https://github.com/SynapticBytes/MiniScript-UDL-for-Notepad-plus-plus/blob/master/MiniScript.xml


    internal static class ClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Keyword")]
        internal static ClassificationTypeDefinition KeywordType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Comment")]
        internal static ClassificationTypeDefinition CommentType = null;

        // Add other types as needed (e.g., Strings, Identifiers, etc.)
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Keyword")]
    [Name("KeywordFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class KeywordFormat : ClassificationFormatDefinition
    {
        public KeywordFormat()
        {
            DisplayName = "Keyword";
            ForegroundColor = Colors.Blue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Comment")]
    [Name("CommentFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class CommentFormat : ClassificationFormatDefinition
    {
        public CommentFormat()
        {
            DisplayName = "Comment";
            ForegroundColor = Colors.Green;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Operator")]
    [Name("OperatorFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class OperatorFormat : ClassificationFormatDefinition
    {
        public OperatorFormat()
        {
            DisplayName = "Operator";
            ForegroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "IntrinsicFunctions")]
    [Name("IntrinsicFunctions")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class IntrinsicFunctions : ClassificationFormatDefinition
    {
        public IntrinsicFunctions()
        {
            DisplayName = "IntrinsicFunctions";
            ForegroundColor = Colors.Green;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "MapType")]
    [Name("MapTypeFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class MapTypeFormat : ClassificationFormatDefinition
    {
        public MapTypeFormat()
        {
            DisplayName = "Map Type";
            ForegroundColor = Colors.Green;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "ListType")]
    [Name("ListTypeFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class ListTypeFormat : ClassificationFormatDefinition
    {
        public ListTypeFormat()
        {
            DisplayName = "List Type";
            ForegroundColor = Colors.Green;
        }
    }

    internal class SyntaxClassifier : IClassifier
    {
        #region 
        private readonly IClassificationType keywordType;
        private readonly IClassificationType commentType;
        private readonly IClassificationType operatorType;
        private readonly IClassificationType mapType;
        private readonly IClassificationType listType;
        private readonly IClassificationType builtinfunctionType;
        #endregion

        internal SyntaxClassifier(IClassificationTypeRegistryService registry)
        {
            keywordType = registry.GetClassificationType("KeywordFormat");
            commentType = registry.GetClassificationType("CommentFormat");
            operatorType = registry.GetClassificationType("OperatorFormat");
            mapType = registry.GetClassificationType("MapType");
            listType = registry.GetClassificationType("ListType");
            builtinfunctionType = registry.GetClassificationType("IntrinsicFunctions");
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var classifications = new List<ClassificationSpan>();

            //if then else|end if|for in|end for|while|end while|function|end function|break|continue
            //abs|acos|asin|atan|ceil|char|cos|floor|log|round|rnd|pi|sign|sin|sqrt|str|tan|indexOf|insert|len|val code remove lower upper replace split hasIndex val code split join push pop pull indexes values sum sort shuffle remove range
            //print time wait locals globals yield _isa input
            //Regex keywordRegex = new Regex(@"\b(if|else|for|while|return)\b", RegexOptions.Compiled);


            Regex keywordRegex = new Regex(@"\b(if |then |else |end if|for in|end for|while|end while|function|end function|break|continue)\b", RegexOptions.Compiled);
            
            //Comments in MiniScript use the // we all know and love
            Regex commentRegex = new Regex(@"//.*$", RegexOptions.Compiled);
            //\w+\[\-?\d+\]|\w+\[:\d+\]|\w+\[\-?\d+:\]|\w+\[\-?\d+:\-?\d+\]
            Regex operatorRegex = new Regex(@"(>|<|<=|>=|=|==|!=|\+|-|%|@|\*|^|/|and|not|or|isa|\w+\[\-?\d+\]|\w+\[:\d+\]|\w+\[\-?\d+:\]|\w+\[\-?\d+:\-?\d+\])", RegexOptions.Compiled);

            //Map (dictionary) syntax for MiniScript
            //\{\s*(?:(?:\d+|".+?")\s*:\s*(?:\d+|".+?")\s*,\s*)*(?:\d+|".+?")\s*:\s*(?:\d+|".+?")\s*\}


            foreach (Match match in keywordRegex.Matches(span.GetText()))
            {
                var matchSpan = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                classifications.Add(new ClassificationSpan(matchSpan, keywordType));
            }

            foreach (Match match in commentRegex.Matches(span.GetText()))
            {
                var matchSpan = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                classifications.Add(new ClassificationSpan(matchSpan, commentType));
            }

            foreach(Match match in operatorRegex.Matches(span.GetText()))
            {
                var matchSpan = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                classifications.Add(new ClassificationSpan(matchSpan, operatorType));
            }

            foreach (Match match in operatorRegex.Matches(span.GetText()))
            {
                var matchSpan = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                classifications.Add(new ClassificationSpan(matchSpan, operatorType));
            }


            return classifications;
        }
    }

    [Export(typeof(IClassifierProvider))]
    [ContentType("text")] // or a specific content type like "csharp" or your custom type
    internal class SyntaxClassifierProvider : IClassifierProvider
    {
        [Import]
        private IClassificationTypeRegistryService classificationRegistry = null;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new SyntaxClassifier(classificationRegistry));
        }
    }

   
}
