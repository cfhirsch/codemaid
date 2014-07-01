﻿#region CodeMaid is Copyright 2007-2014 Steve Cadwallader.

// CodeMaid is free software: you can redistribute it and/or modify it under the terms of the GNU
// Lesser General Public License version 3 as published by the Free Software Foundation.
//
// CodeMaid is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details <http://www.gnu.org/licenses/>.

#endregion CodeMaid is Copyright 2007-2014 Steve Cadwallader.

using EnvDTE;
using SteveCadwallader.CodeMaid.Helpers;
using System;
using System.ComponentModel.Design;
using System.Linq;
using TextSelection = EnvDTE.TextSelection;

namespace SteveCadwallader.CodeMaid.Integration.Commands
{
    /// <summary>
    /// A command that provides for sorting lines.
    /// </summary>
    internal class SortLinesCommand : BaseCommand
    {
        #region Fields

        private readonly UndoTransactionHelper _undoTransactionHelper;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SortLinesCommand" /> class.
        /// </summary>
        /// <param name="package">The hosting package.</param>
        internal SortLinesCommand(CodeMaidPackage package)
            : base(package,
                   new CommandID(GuidList.GuidCodeMaidCommandSortLines, (int)PkgCmdIDList.CmdIDCodeMaidSortLines))
        {
            _undoTransactionHelper = new UndoTransactionHelper(package, "CodeMaid Sort");
        }

        #endregion Constructors

        #region BaseCommand Methods

        /// <summary>
        /// Called to update the current status of the command.
        /// </summary>
        protected override void OnBeforeQueryStatus()
        {
            Enabled = ActiveTextDocument != null;
        }

        /// <summary>
        /// Called to execute the command.
        /// </summary>
        protected override void OnExecute()
        {
            base.OnExecute();

            var activeTextDocument = ActiveTextDocument;
            if (activeTextDocument != null)
            {
                var textSelection = activeTextDocument.Selection;
                if (textSelection != null)
                {
                    _undoTransactionHelper.Run(() => SortText(textSelection));
                }
            }
        }

        #endregion BaseCommand Methods

        #region Properties

        /// <summary>
        /// Gets the active text document, otherwise null.
        /// </summary>
        private TextDocument ActiveTextDocument
        {
            get
            {
                var document = Package.ActiveDocument;

                return document != null ? document.GetTextDocument() : null;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sorts the text within the specified text selection.
        /// </summary>
        /// <param name="textSelection">The text selection.</param>
        private void SortText(TextSelection textSelection)
        {
            // If the selection has no length, try to pick up the next line.
            if (textSelection.IsEmpty)
            {
                textSelection.LineDown(true);
                textSelection.EndOfLine(true);
            }

            //TODO: This is a work in progress.
            System.Diagnostics.Debugger.Break();

            var start = textSelection.TopPoint.CreateEditPoint();
            start.StartOfLine();

            var end = textSelection.BottomPoint.CreateEditPoint();
            end.EndOfLine();

            var text = start.GetText(end);

            var splitText = text.Split(Environment.NewLine.ToArray());
            var orderedText = splitText.OrderBy(x => x);

            start.Delete(end);

            foreach (var line in orderedText)
            {
                start.Insert(line + Environment.NewLine);
            }
        }

        #endregion Methods
    }
}