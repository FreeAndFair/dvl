using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace UI {
    public class FlexibleMessageBox {
      #region Public statics

      /// <summary>
      /// Defines the maximum width for all FlexibleMessageBox instances in percent of the working area.
      /// 
      /// Allowed values are 0.2 - 1.0 where: 
      /// 0.2 means:  The FlexibleMessageBox can be at most half as wide as the working area.
      /// 1.0 means:  The FlexibleMessageBox can be as wide as the working area.
      /// 
      /// Default is: 70% of the working area width.
      /// </summary>
      public static double MAX_WIDTH_FACTOR = 0.7;

      /// <summary>
      /// Defines the maximum height for all FlexibleMessageBox instances in percent of the working area.
      /// 
      /// Allowed values are 0.2 - 1.0 where: 
      /// 0.2 means:  The FlexibleMessageBox can be at most half as high as the working area.
      /// 1.0 means:  The FlexibleMessageBox can be as high as the working area.
      /// 
      /// Default is: 90% of the working area height.
      /// </summary>
      public static double MAX_HEIGHT_FACTOR = 0.9;

      /// <summary>
      /// Defines the font for all FlexibleMessageBox instances.
      /// 
      /// Default is: SystemFonts.MessageBoxFont
      /// </summary>
      public static Font FONT = new Font(SystemFonts.MessageBoxFont.FontFamily.ToString(), 14);

      #endregion

      #region Public show functions

      /// <summary>
      /// Shows the specified message box.
      /// </summary>
      /// <param name="text">The text.</param>
      /// <returns>The dialog result.</returns>
      public static DialogResult Show(string text) {
        return FlexibleMessageBoxForm.Show(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
      }

      /// <summary>
      /// Shows the specified message box.
      /// </summary>
      /// <param name="owner">The owner.</param>
      /// <param name="text">The text.</param>
      /// <returns>The dialog result.</returns>
      public static DialogResult Show(IWin32Window owner, string text) {
        return FlexibleMessageBoxForm.Show(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
      }

      /// <summary>
      /// Shows the specified message box.
      /// </summary>
      /// <param name="text">The text.</param>
      /// <param name="caption">The caption.</param>
      /// <returns>The dialog result.</returns>
      public static DialogResult Show(string text, string caption) {
        return FlexibleMessageBoxForm.Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
      }

      /// <summary>
      /// Shows the specified message box.
      /// </summary>
      /// <param name="owner">The owner.</param>
      /// <param name="text">The text.</param>
      /// <param name="caption">The caption.</param>
      /// <returns>The dialog result.</returns>
      public static DialogResult Show(IWin32Window owner, string text, string caption) {
        return FlexibleMessageBoxForm.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
      }

      /// <summary>
      /// Shows the specified message box.
      /// </summary>
      /// <param name="text">The text.</param>
      /// <param name="caption">The caption.</param>
      /// <param name="buttons">The buttons.</param>
      /// <returns>The dialog result.</returns>
      public static DialogResult Show(string text, string caption, MessageBoxButtons buttons) {
        return FlexibleMessageBoxForm.Show(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
      }

      /// <summary>
      /// Shows the specified message box.
      /// </summary>
      /// <param name="owner">The owner.</param>
      /// <param name="text">The text.</param>
      /// <param name="caption">The caption.</param>
      /// <param name="buttons">The buttons.</param>
      /// <returns>The dialog result.</returns>
      public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) {
        return FlexibleMessageBoxForm.Show(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
      }

      /// <summary>
      /// Shows the specified message box.
      /// </summary>
      /// <param name="text">The text.</param>
      /// <param name="caption">The caption.</param>
      /// <param name="buttons">The buttons.</param>
      /// <param name="icon">The icon.</param>
      /// <returns></returns>
      public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
        return FlexibleMessageBoxForm.Show(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
      }

      /// <summary>
      /// Shows the specified message box.
      /// </summary>
      /// <param name="owner">The owner.</param>
      /// <param name="text">The text.</param>
      /// <param name="caption">The caption.</param>
      /// <param name="buttons">The buttons.</param>
      /// <param name="icon">The icon.</param>
      /// <returns>The dialog result.</returns>
      public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
        return FlexibleMessageBoxForm.Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
      }

      /// <summary>
      /// Shows the specified message box.
      /// </summary>
      /// <param name="text">The text.</param>
      /// <param name="caption">The caption.</param>
      /// <param name="buttons">The buttons.</param>
      /// <param name="icon">The icon.</param>
      /// <param name="defaultButton">The default button.</param>
      /// <returns>The dialog result.</returns>
      public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
        return FlexibleMessageBoxForm.Show(null, text, caption, buttons, icon, defaultButton);
      }

      /// <summary>
      /// Shows the specified message box.
      /// </summary>
      /// <param name="owner">The owner.</param>
      /// <param name="text">The text.</param>
      /// <param name="caption">The caption.</param>
      /// <param name="buttons">The buttons.</param>
      /// <param name="icon">The icon.</param>
      /// <param name="defaultButton">The default button.</param>
      /// <returns>The dialog result.</returns>
      public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
        return FlexibleMessageBoxForm.Show(owner, text, caption, buttons, icon, defaultButton);
      }

      #endregion

      #region Internal form class

      /// <summary>
      /// The form to show the customized message box.
      /// It is defined as an internal class to keep the public interface of the FlexibleMessageBox clean.
      /// </summary>
      class FlexibleMessageBoxForm : Form {
        #region Form-Designer generated code

        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing) {
          if (disposing && (components != null)) {
            components.Dispose();
          }
          base.Dispose(disposing);
        }

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent() {
          components = new System.ComponentModel.Container();
          button1 = new System.Windows.Forms.Button();
          richTextBoxMessage = new System.Windows.Forms.RichTextBox();
          FlexibleMessageBoxFormBindingSource = new System.Windows.Forms.BindingSource(components);
          panel1 = new System.Windows.Forms.Panel();
          pictureBoxForIcon = new System.Windows.Forms.PictureBox();
          button2 = new System.Windows.Forms.Button();
          button3 = new System.Windows.Forms.Button();
          ((System.ComponentModel.ISupportInitialize)(FlexibleMessageBoxFormBindingSource)).BeginInit();
          panel1.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(pictureBoxForIcon)).BeginInit();
          SuspendLayout();
          // 
          // button1
          // 
          button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
          button1.AutoSize = true;
          button1.DialogResult = System.Windows.Forms.DialogResult.OK;
          button1.Location = new System.Drawing.Point(11, 67);
          button1.MinimumSize = new System.Drawing.Size(0, 24);
          button1.Name = "button1";
          button1.Size = new System.Drawing.Size(75, 24);
          button1.TabIndex = 2;
          button1.Text = "OK";
          button1.UseVisualStyleBackColor = true;
          button1.Visible = false;
          // 
          // richTextBoxMessage
          // 
          richTextBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
          | System.Windows.Forms.AnchorStyles.Left)
          | System.Windows.Forms.AnchorStyles.Right)));
          richTextBoxMessage.BackColor = System.Drawing.Color.White;
          richTextBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
          richTextBoxMessage.DataBindings.Add(new System.Windows.Forms.Binding("Text", FlexibleMessageBoxFormBindingSource, "MessageText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
          richTextBoxMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          richTextBoxMessage.Location = new System.Drawing.Point(50, 26);
          richTextBoxMessage.Margin = new System.Windows.Forms.Padding(0);
          richTextBoxMessage.Name = "richTextBoxMessage";
          richTextBoxMessage.ReadOnly = true;
          richTextBoxMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
          richTextBoxMessage.Size = new System.Drawing.Size(200, 20);
          richTextBoxMessage.TabIndex = 0;
          richTextBoxMessage.TabStop = false;
          richTextBoxMessage.Text = "<Message>";
          richTextBoxMessage.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(richTextBoxMessage_LinkClicked);
          // 
          // panel1
          // 
          panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
          | System.Windows.Forms.AnchorStyles.Left)
          | System.Windows.Forms.AnchorStyles.Right)));
          panel1.BackColor = System.Drawing.Color.White;
          panel1.Controls.Add(pictureBoxForIcon);
          panel1.Controls.Add(richTextBoxMessage);
          panel1.Location = new System.Drawing.Point(-3, -4);
          panel1.Name = "panel1";
          panel1.Size = new System.Drawing.Size(268, 59);
          panel1.TabIndex = 1;
          // 
          // pictureBoxForIcon
          // 
          pictureBoxForIcon.BackColor = System.Drawing.Color.Transparent;
          pictureBoxForIcon.Location = new System.Drawing.Point(15, 19);
          pictureBoxForIcon.Name = "pictureBoxForIcon";
          pictureBoxForIcon.Size = new System.Drawing.Size(32, 32);
          pictureBoxForIcon.TabIndex = 8;
          pictureBoxForIcon.TabStop = false;
          // 
          // button2
          // 
          button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
          button2.DialogResult = System.Windows.Forms.DialogResult.OK;
          button2.Location = new System.Drawing.Point(92, 67);
          button2.MinimumSize = new System.Drawing.Size(0, 24);
          button2.Name = "button2";
          button2.Size = new System.Drawing.Size(75, 24);
          button2.TabIndex = 3;
          button2.Text = "OK";
          button2.UseVisualStyleBackColor = true;
          button2.Visible = false;
          // 
          // button3
          // 
          button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
          button3.AutoSize = true;
          button3.DialogResult = System.Windows.Forms.DialogResult.OK;
          button3.Location = new System.Drawing.Point(173, 67);
          button3.MinimumSize = new System.Drawing.Size(0, 24);
          button3.Name = "button3";
          button3.Size = new System.Drawing.Size(75, 24);
          button3.TabIndex = 0;
          button3.Text = "OK";
          button3.UseVisualStyleBackColor = true;
          button3.Visible = false;
          // 
          // FlexibleMessageBoxForm
          // 
          AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          ClientSize = new System.Drawing.Size(260, 102);
          Controls.Add(button3);
          Controls.Add(button2);
          Controls.Add(panel1);
          Controls.Add(button1);
          DataBindings.Add(new System.Windows.Forms.Binding("Text", FlexibleMessageBoxFormBindingSource, "CaptionText", true));
          MaximizeBox = false;
          MinimizeBox = false;
          MinimumSize = new System.Drawing.Size(276, 140);
          Name = "FlexibleMessageBoxForm";
          ShowIcon = false;
          SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
          StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
          Text = "<Caption>";
          Shown += new System.EventHandler(FlexibleMessageBoxForm_Shown);
          ((System.ComponentModel.ISupportInitialize)(FlexibleMessageBoxFormBindingSource)).EndInit();
          panel1.ResumeLayout(false);
          ((System.ComponentModel.ISupportInitialize)(pictureBoxForIcon)).EndInit();
          ResumeLayout(false);
          PerformLayout();
        }

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.BindingSource FlexibleMessageBoxFormBindingSource;
        private System.Windows.Forms.RichTextBox richTextBoxMessage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBoxForIcon;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;

        #endregion

        #region Private constants

        //These separators are used for the "copy to clipboard" standard operation, triggered by Ctrl + C (behavior and clipboard format is like in a standard MessageBox)
        private static readonly String STANDARD_MESSAGEBOX_SEPARATOR_LINES = "---------------------------\n";
        private static readonly String STANDARD_MESSAGEBOX_SEPARATOR_SPACES = "   ";

        //These are the possible buttons (in a standard MessageBox)
        private enum ButtonID { OK = 0, CANCEL, YES, NO, ABORT, RETRY, IGNORE };

        //These are the buttons texts for different languages. 
        //If you want to add a new language, add it here and in the GetButtonText-Function
        private enum TwoLetterISOLanguageID { en, de, es, it };
        private static readonly String[] BUTTON_TEXTS_ENGLISH_EN = { "OK", "Cancel", "&Yes", "&No", "&Abort", "&Retry", "&Ignore" }; //Note: This is also the fallback language
        private static readonly String[] BUTTON_TEXTS_GERMAN_DE = { "OK", "Abbrechen", "&Ja", "&Nein", "&Abbrechen", "&Wiederholen", "&Ignorieren" };
        private static readonly String[] BUTTON_TEXTS_SPANISH_ES = { "Aceptar", "Cancelar", "&Sí", "&No", "&Abortar", "&Reintentar", "&Ignorar" };
        private static readonly String[] BUTTON_TEXTS_ITALIAN_IT = { "OK", "Annulla", "&Sì", "&No", "&Interrompi", "&Riprova", "&Ignora" };

        #endregion

        #region Private members

        private MessageBoxDefaultButton defaultButton;
        private int visibleButtonsCount;
        private TwoLetterISOLanguageID languageID = TwoLetterISOLanguageID.en;

        #endregion

        #region Private constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexibleMessageBoxForm"/> class.
        /// </summary>
        private FlexibleMessageBoxForm() {
          InitializeComponent();

          //Try to evaluate the language. If this fails, the fallback language English will be used
          Enum.TryParse<TwoLetterISOLanguageID>(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, out languageID);

          KeyPreview = true;
          KeyUp += FlexibleMessageBoxForm_KeyUp;
        }

        #endregion

        #region Private helper functions

        /// <summary>
        /// Gets the string rows.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The string rows as 1-dimensional array</returns>
        private static string[] GetStringRows(string message) {
          if (string.IsNullOrEmpty(message)) return null;

          var messageRows = message.Split(new char[] { '\n' }, StringSplitOptions.None);
          return messageRows;
        }

        /// <summary>
        /// Gets the button text for the CurrentUICulture language.
        /// Note: The fallback language is English
        /// </summary>
        /// <param name="buttonID">The ID of the button.</param>
        /// <returns>The button text</returns>
        private string GetButtonText(ButtonID buttonID) {
          var buttonTextArrayIndex = Convert.ToInt32(buttonID);

          switch (languageID) {
            case TwoLetterISOLanguageID.de: return BUTTON_TEXTS_GERMAN_DE[buttonTextArrayIndex];
            case TwoLetterISOLanguageID.es: return BUTTON_TEXTS_SPANISH_ES[buttonTextArrayIndex];
            case TwoLetterISOLanguageID.it: return BUTTON_TEXTS_ITALIAN_IT[buttonTextArrayIndex];

            default: return BUTTON_TEXTS_ENGLISH_EN[buttonTextArrayIndex];
          }
        }

        /// <summary>
        /// Ensure the given working area factor in the range of  0.2 - 1.0 where: 
        /// 
        /// 0.2 means:  20 percent of the working area height or width.
        /// 1.0 means:  100 percent of the working area height or width.
        /// </summary>
        /// <param name="workingAreaFactor">The given working area factor.</param>
        /// <returns>The corrected given working area factor.</returns>
        private static double GetCorrectedWorkingAreaFactor(double workingAreaFactor) {
          const double MIN_FACTOR = 0.2;
          const double MAX_FACTOR = 1.0;

          if (workingAreaFactor < MIN_FACTOR) return MIN_FACTOR;
          if (workingAreaFactor > MAX_FACTOR) return MAX_FACTOR;

          return workingAreaFactor;
        }

        /// <summary>
        /// Set the dialogs start position when given. 
        /// Otherwise center the dialog on the current screen.
        /// </summary>
        /// <param name="flexibleMessageBoxForm">The FlexibleMessageBox dialog.</param>
        /// <param name="owner">The owner.</param>
        private static void SetDialogStartPosition(FlexibleMessageBoxForm flexibleMessageBoxForm, IWin32Window owner) {
          //If no owner given: Center on current screen
          if (owner == null) {
            var screen = Screen.FromPoint(Cursor.Position);
            flexibleMessageBoxForm.StartPosition = FormStartPosition.Manual;
            flexibleMessageBoxForm.Left = screen.Bounds.Left + screen.Bounds.Width / 2 - flexibleMessageBoxForm.Width / 2;
            flexibleMessageBoxForm.Top = screen.Bounds.Top + screen.Bounds.Height / 2 - flexibleMessageBoxForm.Height / 2;
          }
        }

        /// <summary>
        /// Calculate the dialogs start size (Try to auto-size width to show longest text row).
        /// Also set the maximum dialog size. 
        /// </summary>
        /// <param name="flexibleMessageBoxForm">The FlexibleMessageBox dialog.</param>
        /// <param name="text">The text (the longest text row is used to calculate the dialog width).</param>
        /// <param name="text">The caption (this can also affect the dialog width).</param>
        private static void SetDialogSizes(FlexibleMessageBoxForm flexibleMessageBoxForm, string text, string caption, IWin32Window owner) {
          //First set the bounds for the maximum dialog size
          flexibleMessageBoxForm.MaximumSize = new Size(Convert.ToInt32(SystemInformation.WorkingArea.Width * FlexibleMessageBoxForm.GetCorrectedWorkingAreaFactor(MAX_WIDTH_FACTOR)),
                                                        Convert.ToInt32(SystemInformation.WorkingArea.Height * FlexibleMessageBoxForm.GetCorrectedWorkingAreaFactor(MAX_HEIGHT_FACTOR)));
          // maximum size as big as owner window
          //RECT rectForm = new RECT();
          //IntPtr ownerPtr = owner.Handle;
          //if (ownerPtr == null) ownerPtr = USER32.GetActiveWindow();
          //USER32.GetWindowRect(ownerPtr, ref rectForm); 
          //flexibleMessageBoxForm.MaximumSize = new Size(Convert.ToInt32((rectForm.right - rectForm.left)*FlexibleMessageBoxForm.GetCorrectedWorkingAreaFactor(MAX_WIDTH_FACTOR)),
          //                                    Convert.ToInt32((rectForm.bottom - rectForm.top * FlexibleMessageBoxForm.GetCorrectedWorkingAreaFactor(MAX_HEIGHT_FACTOR)));

          //Get rows. Exit if there are no rows to render...
          var stringRows = GetStringRows(text);
          if (stringRows == null) return;

          //Calculate whole text height
          var textHeight = TextRenderer.MeasureText(text, FONT).Height;

          //Calculate width for longest text line
          const int SCROLLBAR_WIDTH_OFFSET = 15;
          var longestTextRowWidth = stringRows.Max(textForRow => TextRenderer.MeasureText(textForRow, FONT).Width);
          var captionWidth = TextRenderer.MeasureText(caption, SystemFonts.CaptionFont).Width;
          var textWidth = Math.Max(longestTextRowWidth + SCROLLBAR_WIDTH_OFFSET, captionWidth);

          //Calculate margins
          var marginWidth = flexibleMessageBoxForm.Width - flexibleMessageBoxForm.richTextBoxMessage.Width;
          var marginHeight = flexibleMessageBoxForm.Height - flexibleMessageBoxForm.richTextBoxMessage.Height;

          //Set calculated dialog size (if the calculated values exceed the maximums, they were cut by windows forms automatically)
          flexibleMessageBoxForm.Size = new Size(textWidth + marginWidth,
                                                 textHeight + marginHeight);
        }

        /// <summary>
        /// Set the dialogs icon. 
        /// When no icon is used: Correct placement and width of rich text box.
        /// </summary>
        /// <param name="flexibleMessageBoxForm">The FlexibleMessageBox dialog.</param>
        /// <param name="icon">The MessageBoxIcon.</param>
        private static void SetDialogIcon(FlexibleMessageBoxForm flexibleMessageBoxForm, MessageBoxIcon icon) {
          switch (icon) {
            case MessageBoxIcon.Information:
              flexibleMessageBoxForm.pictureBoxForIcon.Image = SystemIcons.Information.ToBitmap();
              break;
            case MessageBoxIcon.Warning:
              flexibleMessageBoxForm.pictureBoxForIcon.Image = SystemIcons.Warning.ToBitmap();
              break;
            case MessageBoxIcon.Error:
              flexibleMessageBoxForm.pictureBoxForIcon.Image = SystemIcons.Error.ToBitmap();
              break;
            case MessageBoxIcon.Question:
              flexibleMessageBoxForm.pictureBoxForIcon.Image = SystemIcons.Question.ToBitmap();
              break;
            default:
              //When no icon is used: Correct placement and width of rich text box.
              flexibleMessageBoxForm.pictureBoxForIcon.Visible = false;
              flexibleMessageBoxForm.richTextBoxMessage.Left -= flexibleMessageBoxForm.pictureBoxForIcon.Width;
              flexibleMessageBoxForm.richTextBoxMessage.Width += flexibleMessageBoxForm.pictureBoxForIcon.Width;
              break;
          }
        }

        /// <summary>
        /// Set dialog buttons visibilities and texts. 
        /// Also set a default button.
        /// </summary>
        /// <param name="flexibleMessageBoxForm">The FlexibleMessageBox dialog.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="defaultButton">The default button.</param>
        private static void SetDialogButtons(FlexibleMessageBoxForm flexibleMessageBoxForm, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
          //Set the buttons visibilities and texts
          switch (buttons) {
            case MessageBoxButtons.AbortRetryIgnore:
              flexibleMessageBoxForm.visibleButtonsCount = 3;

              flexibleMessageBoxForm.button1.Visible = true;
              flexibleMessageBoxForm.button1.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.ABORT);
              flexibleMessageBoxForm.button1.DialogResult = DialogResult.Abort;

              flexibleMessageBoxForm.button2.Visible = true;
              flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.RETRY);
              flexibleMessageBoxForm.button2.DialogResult = DialogResult.Retry;

              flexibleMessageBoxForm.button3.Visible = true;
              flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.IGNORE);
              flexibleMessageBoxForm.button3.DialogResult = DialogResult.Ignore;

              flexibleMessageBoxForm.ControlBox = false;
              break;

            case MessageBoxButtons.OKCancel:
              flexibleMessageBoxForm.visibleButtonsCount = 2;

              flexibleMessageBoxForm.button2.Visible = true;
              flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.OK);
              flexibleMessageBoxForm.button2.DialogResult = DialogResult.OK;

              flexibleMessageBoxForm.button3.Visible = true;
              flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.CANCEL);
              flexibleMessageBoxForm.button3.DialogResult = DialogResult.Cancel;

              flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm.button3;
              break;

            case MessageBoxButtons.RetryCancel:
              flexibleMessageBoxForm.visibleButtonsCount = 2;

              flexibleMessageBoxForm.button2.Visible = true;
              flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.RETRY);
              flexibleMessageBoxForm.button2.DialogResult = DialogResult.Retry;

              flexibleMessageBoxForm.button3.Visible = true;
              flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.CANCEL);
              flexibleMessageBoxForm.button3.DialogResult = DialogResult.Cancel;

              flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm.button3;
              break;

            case MessageBoxButtons.YesNo:
              flexibleMessageBoxForm.visibleButtonsCount = 2;

              flexibleMessageBoxForm.button2.Visible = true;
              flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.YES);
              flexibleMessageBoxForm.button2.DialogResult = DialogResult.Yes;

              flexibleMessageBoxForm.button3.Visible = true;
              flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.NO);
              flexibleMessageBoxForm.button3.DialogResult = DialogResult.No;

              flexibleMessageBoxForm.ControlBox = false;
              break;

            case MessageBoxButtons.YesNoCancel:
              flexibleMessageBoxForm.visibleButtonsCount = 3;

              flexibleMessageBoxForm.button1.Visible = true;
              flexibleMessageBoxForm.button1.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.YES);
              flexibleMessageBoxForm.button1.DialogResult = DialogResult.Yes;

              flexibleMessageBoxForm.button2.Visible = true;
              flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.NO);
              flexibleMessageBoxForm.button2.DialogResult = DialogResult.No;

              flexibleMessageBoxForm.button3.Visible = true;
              flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.CANCEL);
              flexibleMessageBoxForm.button3.DialogResult = DialogResult.Cancel;

              flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm.button3;
              break;

            case MessageBoxButtons.OK:
            default:
              flexibleMessageBoxForm.visibleButtonsCount = 1;
              flexibleMessageBoxForm.button3.Visible = true;
              flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonID.OK);
              flexibleMessageBoxForm.button3.DialogResult = DialogResult.OK;

              flexibleMessageBoxForm.CancelButton = flexibleMessageBoxForm.button3;
              break;
          }

          //Set default button (used in FlexibleMessageBoxForm_Shown)
          flexibleMessageBoxForm.defaultButton = defaultButton;
        }

        #endregion

        #region Private event handlers

        /// <summary>
        /// Handles the Shown event of the FlexibleMessageBoxForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void FlexibleMessageBoxForm_Shown(object sender, EventArgs e) {
          int buttonIndexToFocus = 1;
          Button buttonToFocus;

          //Set the default button...
          switch (defaultButton) {
            case MessageBoxDefaultButton.Button1:
            default:
              buttonIndexToFocus = 1;
              break;
            case MessageBoxDefaultButton.Button2:
              buttonIndexToFocus = 2;
              break;
            case MessageBoxDefaultButton.Button3:
              buttonIndexToFocus = 3;
              break;
          }

          if (buttonIndexToFocus > visibleButtonsCount) buttonIndexToFocus = visibleButtonsCount;

          if (buttonIndexToFocus == 3) {
            buttonToFocus = button3;
          } else if (buttonIndexToFocus == 2) {
            buttonToFocus = button2;
          } else {
            buttonToFocus = button1;
          }

          buttonToFocus.Focus();
        }

        /// <summary>
        /// Handles the LinkClicked event of the richTextBoxMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkClickedEventArgs"/> instance containing the event data.</param>
        private void richTextBoxMessage_LinkClicked(object sender, LinkClickedEventArgs e) {
          try {
            Cursor.Current = Cursors.WaitCursor;
            Process.Start(e.LinkText);
          } catch (Exception) {
            //Let the caller of FlexibleMessageBoxForm decide what to do with this exception...
            throw;
          } finally {
            Cursor.Current = Cursors.Default;
          }

        }

        /// <summary>
        /// Handles the KeyUp event of the richTextBoxMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        void FlexibleMessageBoxForm_KeyUp(object sender, KeyEventArgs e) {
          //Handle standard key strikes for clipboard copy: "Ctrl + C" and "Ctrl + Insert"
          if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.Insert)) {
            var buttonsTextLine = (button1.Visible ? button1.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty)
                                + (button2.Visible ? button2.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty)
                                + (button3.Visible ? button3.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty);

            //Build same clipboard text like the standard .Net MessageBox
            var textForClipboard = STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                 + Text + Environment.NewLine
                                 + STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                 + richTextBoxMessage.Text + Environment.NewLine
                                 + STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                 + buttonsTextLine.Replace("&", string.Empty) + Environment.NewLine
                                 + STANDARD_MESSAGEBOX_SEPARATOR_LINES;

            //Set text in clipboard
            Clipboard.SetText(textForClipboard);
          }
        }

        #endregion

        #region Properties (only used for binding)

        /// <summary>
        /// The text that is been used for the heading.
        /// </summary>
        public string CaptionText { get; set; }

        /// <summary>
        /// The text that is been used in the FlexibleMessageBoxForm.
        /// </summary>
        public string MessageText { get; set; }

        #endregion

        #region Public show function

        /// <summary>
        /// Shows the specified message box.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns>The dialog result.</returns>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
          //Create a new instance of the FlexibleMessageBox form
          var flexibleMessageBoxForm = new FlexibleMessageBoxForm();
          flexibleMessageBoxForm.ShowInTaskbar = false;

          //Bind the caption and the message text
          flexibleMessageBoxForm.CaptionText = caption;
          flexibleMessageBoxForm.MessageText = text;
          flexibleMessageBoxForm.FlexibleMessageBoxFormBindingSource.DataSource = flexibleMessageBoxForm;

          //Set the buttons visibilities and texts. Also set a default button.
          SetDialogButtons(flexibleMessageBoxForm, buttons, defaultButton);

          //Set the dialogs icon. When no icon is used: Correct placement and width of rich text box.
          SetDialogIcon(flexibleMessageBoxForm, icon);

          //Set the font for all controls
          flexibleMessageBoxForm.Font = FONT;
          flexibleMessageBoxForm.richTextBoxMessage.Font = FONT;

          //Calculate the dialogs start size (Try to auto-size width to show longest text row). Also set the maximum dialog size. 
          SetDialogSizes(flexibleMessageBoxForm, text, caption, owner);

          //Set the dialogs start position when given. Otherwise center the dialog on the current screen.
          SetDialogStartPosition(flexibleMessageBoxForm, owner);

          //Show the dialog
          CenterWindow centerWindow = new CenterWindow(owner.Handle);
          DialogResult dlgResult = flexibleMessageBoxForm.ShowDialog(owner);
          centerWindow.Dispose();
          return dlgResult;
        }

        #endregion
      } //class FlexibleMessageBoxForm

      #endregion
    }

    // Code below this point is from CodeProject's 
    // "Centering MessageBox, Common DialogBox or Form on applications"
    // Copyright (C) Jean-Claude Lanz, 2005

    ///////////////////////////////////////////////////////////////////////
    #region CenterWindow class

    public class CenterWindow {
      public IntPtr hOwner = IntPtr.Zero;
      private Rectangle rect;

      public CbtHook cbtHook = null;
      public WndProcRetHook wndProcRetHook = null;

      public CenterWindow(IntPtr hOwner) {
        this.hOwner = hOwner;
        this.cbtHook = new CbtHook();
        cbtHook.WindowActivate += new CbtHook.CbtEventHandler(WndActivate);
        cbtHook.Install();
      }

      public void Dispose() {
        if (wndProcRetHook != null) {
          wndProcRetHook.Uninstall();
          wndProcRetHook = null;
        }
        if (cbtHook != null) {
          cbtHook.Uninstall();
          cbtHook = null;
        }
      }

      public void WndActivate(object sender, CbtEventArgs e) {
        IntPtr hMsgBox = e.wParam;

        // try to find a howner for this message box
        if (hOwner == IntPtr.Zero) {
          Console.WriteLine("Using active window as hOwner because hOwner is Zero");
          hOwner = USER32.GetActiveWindow();
        }
        // get the MessageBox window rect
        RECT rectDlg = new RECT();
        USER32.GetWindowRect(hMsgBox, ref rectDlg);

        // get the owner window rect
        RECT rectForm = new RECT();
        USER32.GetWindowRect(hOwner, ref rectForm);

        Console.WriteLine("Rectangle Coordinates: " + rectForm.left + " " + rectForm.right + " " + rectForm.top + " " + rectForm.bottom);
        // get the biggest screen area
        Rectangle rectScreen = API.TrueScreenRect;

        // if no parent window, center on the primary screen
        if (rectForm.right == rectForm.left)
          rectForm.right = rectForm.left = Screen.PrimaryScreen.WorkingArea.Width / 2;
        if (rectForm.bottom == rectForm.top)
          rectForm.bottom = rectForm.top = Screen.PrimaryScreen.WorkingArea.Height / 2;

        // center on parent
        int dx = ((rectDlg.left + rectDlg.right) - (rectForm.left + rectForm.right)) / 2;
        int dy = ((rectDlg.top + rectDlg.bottom) - (rectForm.top + rectForm.bottom)) / 2;

        rect = new Rectangle(
          rectDlg.left - dx,
          rectDlg.top - dy,
          rectDlg.right - rectDlg.left,
          rectDlg.bottom - rectDlg.top);

        // place in the screen
        if (rect.Right > rectScreen.Right) rect.Offset(rectScreen.Right - rect.Right, 0);
        if (rect.Bottom > rectScreen.Bottom) rect.Offset(0, rectScreen.Bottom - rect.Bottom);
        if (rect.Left < rectScreen.Left) rect.Offset(rectScreen.Left - rect.Left, 0);
        if (rect.Top < rectScreen.Top) rect.Offset(0, rectScreen.Top - rect.Top);

        if (e.IsDialog) {
          // do the job when the WM_INITDIALOG message returns
          wndProcRetHook = new WndProcRetHook(hMsgBox);
          wndProcRetHook.WndProcRet += new WndProcRetHook.WndProcEventHandler(WndProcRet);
          wndProcRetHook.Install();
        } else
          USER32.MoveWindow(hMsgBox, rect.Left, rect.Top, rect.Width, rect.Height, 1);

        // uninstall this hook
        WindowsHook wndHook = (WindowsHook)sender;
        Debug.Assert(cbtHook == wndHook);
        cbtHook.Uninstall();
        cbtHook = null;
      }

      public void WndProcRet(object sender, WndProcRetEventArgs e) {
        if (e.cw.message == WndMessage.WM_INITDIALOG ||
          e.cw.message == WndMessage.WM_UNKNOWINIT) {
          USER32.MoveWindow(e.cw.hwnd, rect.Left, rect.Top, rect.Width, rect.Height, 1);

          // uninstall this hook
          WindowsHook wndHook = (WindowsHook)sender;
          Debug.Assert(wndProcRetHook == wndHook);
          wndProcRetHook.Uninstall();
          wndProcRetHook = null;
        }
      }
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////
    #region Generic declarations

    /// <summary>
    /// Rectangle parameters exposed as a structure.
    /// </summary>
    public struct RECT {
      /// <summary>
      /// Rectangle members.
      /// </summary>
      public int left, top, right, bottom;
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////
    #region Util class

    /// <summary>
    /// Utility functions.
    /// </summary>
    public sealed class API {
      private API() { }	// To remove the constructor from the documentation!

      /// <summary>
      /// Get true multiscreen size.
      /// </summary>
      public static Rectangle TrueScreenRect {
        get {
          // get the biggest screen area
          Rectangle rectScreen = Screen.PrimaryScreen.WorkingArea;
          int left = rectScreen.Left;
          int top = rectScreen.Top;
          int right = rectScreen.Right;
          int bottom = rectScreen.Bottom;
          foreach (Screen screen in Screen.AllScreens) {
            left = Math.Min(left, screen.WorkingArea.Left);
            right = Math.Max(right, screen.WorkingArea.Right);
            top = Math.Min(top, screen.WorkingArea.Top);
            bottom = Math.Max(bottom, screen.WorkingArea.Bottom);
          }
          return new Rectangle(left, top, right - left, bottom - top);
        }
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////
    #region USER32 class

    /// <summary>
    /// Class to expose USER32 API functions.
    /// </summary>
    public sealed class USER32 {
      private USER32() { }	// To remove the constructor from the documentation!

      [DllImport("user32", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
      internal static extern int GetWindowRect(IntPtr hWnd, ref RECT rect);

      [DllImport("user32", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
      internal static extern int MoveWindow(IntPtr hWnd, int x, int y, int w, int h, int repaint);

      [DllImport("user32", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
      internal static extern IntPtr GetActiveWindow();

      [DllImport("user32", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
      internal static extern int GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);
    }
    #endregion

    #region Class HookEventArgs

    /// Class used for hook event arguments.
    public class HookEventArgs : EventArgs {
      /// Event code parameter.
      public int code;
      /// wParam parameter.
      public IntPtr wParam;
      /// lParam parameter.
      public IntPtr lParam;

      internal HookEventArgs(int code, IntPtr wParam, IntPtr lParam) {
        this.code = code;
        this.wParam = wParam;
        this.lParam = lParam;
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////
    #region Enum HookType

    /// Hook Types.
    public enum HookType : int {
      /// <value>0</value>
      WH_JOURNALRECORD = 0,
      /// <value>1</value>
      WH_JOURNALPLAYBACK = 1,
      /// <value>2</value>
      WH_KEYBOARD = 2,
      /// <value>3</value>
      WH_GETMESSAGE = 3,
      /// <value>4</value>
      WH_CALLWNDPROC = 4,
      /// <value>5</value>
      WH_CBT = 5,
      /// <value>6</value>
      WH_SYSMSGFILTER = 6,
      /// <value>7</value>
      WH_MOUSE = 7,
      /// <value>8</value>
      WH_HARDWARE = 8,
      /// <value>9</value>
      WH_DEBUG = 9,
      /// <value>10</value>
      WH_SHELL = 10,
      /// <value>11</value>
      WH_FOREGROUNDIDLE = 11,
      /// <value>12</value>
      WH_CALLWNDPROCRET = 12,
      /// <value>13</value>
      WH_KEYBOARD_LL = 13,
      /// <value>14</value>
      WH_MOUSE_LL = 14
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////
    #region Class WindowsHook

    /// <summary>
    /// Class to expose the windows hook mechanism.
    /// </summary>
    public class WindowsHook {
      /// <summary>
      /// Hook delegate method.
      /// </summary>
      public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

      // internal properties
      internal IntPtr hHook = IntPtr.Zero;
      internal HookProc filterFunc = null;
      internal HookType hookType;

      /// <summary>
      /// Hook delegate method.
      /// </summary>
      public delegate void HookEventHandler(object sender, HookEventArgs e);

      /// <summary>
      /// Hook invoke event.
      /// </summary>
      public event HookEventHandler HookInvoke;

      internal void OnHookInvoke(HookEventArgs e) {
        if (HookInvoke != null)
          HookInvoke(this, e);
      }

      /// <summary>
      /// Construct a HookType hook.
      /// </summary>
      /// <param name="hook">Hook type.</param>
      public WindowsHook(HookType hook) {
        hookType = hook;
        filterFunc = new HookProc(this.CoreHookProc);
      }
      /// <summary>
      /// Construct a HookType hook giving a hook filter delegate method.
      /// </summary>
      /// <param name="hook">Hook type</param>
      /// <param name="func">Hook filter event.</param>
      public WindowsHook(HookType hook, HookProc func) {
        hookType = hook;
        filterFunc = func;
      }

      // default hook filter function
      internal int CoreHookProc(int code, IntPtr wParam, IntPtr lParam) {
        if (code < 0)
          return CallNextHookEx(hHook, code, wParam, lParam);

        // let clients determine what to do
        HookEventArgs e = new HookEventArgs(code, wParam, lParam);
        OnHookInvoke(e);

        // yield to the next hook in the chain
        return CallNextHookEx(hHook, code, wParam, lParam);
      }

      /// <summary>
      /// Install the hook. 
      /// </summary>
      public void Install() {
        hHook = SetWindowsHookEx(hookType, filterFunc, IntPtr.Zero, (int)AppDomain.GetCurrentThreadId());
      }


      /// <summary>
      /// Uninstall the hook.
      /// </summary>
      public void Uninstall() {
        if (hHook != IntPtr.Zero) {
          UnhookWindowsHookEx(hHook);
          hHook = IntPtr.Zero;
        }
      }

      #region Win32 Imports

      [DllImport("user32.dll")]
      internal static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr hInstance, int threadID);

      [DllImport("user32.dll")]
      internal static extern int UnhookWindowsHookEx(IntPtr hhook);

      [DllImport("user32.dll")]
      internal static extern int CallNextHookEx(IntPtr hhook, int code, IntPtr wParam, IntPtr lParam);

      #endregion
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////
    #region Enum WndMessage

    /// <summary>
    /// windows message.
    /// </summary>
    public enum WndMessage : int {
      /// Sent to the dialog procedure immediately before the dialog is displayed.
      WM_INITDIALOG = 0x0110,
      /// Sent to the dialog procedure immediately before the dialog is displayed.
      WM_UNKNOWINIT = 0x0127
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////
    #region Class WndProcRetEventArgs

    /// Class used for WH_CALLWNDPROCRET hook event arguments.
    public class WndProcRetEventArgs : EventArgs {
      /// wParam parameter.
      public IntPtr wParam;
      /// lParam parameter.
      public IntPtr lParam;
      /// CWPRETSTRUCT structure.
      public CwPRetStruct cw;

      internal WndProcRetEventArgs(IntPtr wParam, IntPtr lParam) {
        this.wParam = wParam;
        this.lParam = lParam;
        cw = new CwPRetStruct();
        Marshal.PtrToStructure(lParam, cw);
      }
    }

    /// <summary>
    /// CWPRETSTRUCT structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class CwPRetStruct {
      /// Return value.
      public int lResult;
      /// lParam parameter.
      public int lParam;
      /// wParam parameter.
      public int wParam;
      /// Specifies the message.
      public WndMessage message;
      /// Handle to the window that processed the message.
      public IntPtr hwnd;
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////
    #region Class WndProcRetHook

    /// <summary>
    /// Class to expose the windows WH_CALLWNDPROCRET hook mechanism.
    /// </summary>
    public class WndProcRetHook : WindowsHook {
      /// <summary>
      /// WH_CALLWNDPROCRET hook delegate method.
      /// </summary>
      public delegate void WndProcEventHandler(object sender, WndProcRetEventArgs e);

      private IntPtr hWndHooked;

      /// <summary>
      /// Window procedure event.
      /// </summary>
      public event WndProcEventHandler WndProcRet;

      /// <summary>
      /// Construct a WH_CALLWNDPROCRET hook.
      /// </summary>
      /// <param name="hWndHooked">
      /// Handle of the window to be hooked. IntPtr.Zero to hook all window.
      /// </param>
      public WndProcRetHook(IntPtr hWndHooked)
        : base(HookType.WH_CALLWNDPROCRET) {
        this.hWndHooked = hWndHooked;
        this.HookInvoke += new HookEventHandler(WndProcRetHookInvoked);
      }
      /// <summary>
      /// Construct a WH_CALLWNDPROCRET hook giving a hook filter delegate method.
      /// </summary>
      /// <param name="hWndHooked">
      /// Handle of the window to be hooked. IntPtr.Zero to hook all window.
      /// </param>
      /// <param name="func">Hook filter event.</param>
      public WndProcRetHook(IntPtr hWndHooked, HookProc func)
        : base(HookType.WH_CALLWNDPROCRET, func) {
        this.hWndHooked = hWndHooked;
        this.HookInvoke += new HookEventHandler(WndProcRetHookInvoked);
      }

      // handles the hook event
      private void WndProcRetHookInvoked(object sender, HookEventArgs e) {
        WndProcRetEventArgs wpe = new WndProcRetEventArgs(e.wParam, e.lParam);
        if ((hWndHooked == IntPtr.Zero || wpe.cw.hwnd == hWndHooked) && WndProcRet != null)
          WndProcRet(this, wpe);
        return;
      }
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////
    #region Enum CbtHookAction

    /// <summary>
    /// CBT hook actions.
    /// </summary>
    internal enum CbtHookAction : int {
      HCBT_MOVESIZE = 0,
      HCBT_MINMAX = 1,
      HCBT_QS = 2,
      HCBT_CREATEWND = 3,
      HCBT_DESTROYWND = 4,
      HCBT_ACTIVATE = 5,
      HCBT_CLICKSKIPPED = 6,
      HCBT_KEYSKIPPED = 7,
      HCBT_SYSCOMMAND = 8,
      HCBT_SETFOCUS = 9
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////
    #region Class CbtEventArgs

    /// <summary>
    /// Class used for WH_CBT hook event arguments.
    /// </summary>
    public class CbtEventArgs : EventArgs {
      /// wParam parameter.
      public IntPtr wParam;
      /// lParam parameter.
      public IntPtr lParam;
      /// Window class name.
      public string className;
      /// True if it is a dialog window.
      public bool IsDialog;

      internal CbtEventArgs(IntPtr wParam, IntPtr lParam) {
        // cache the parameters
        this.wParam = wParam;
        this.lParam = lParam;

        // cache the window's class name
        StringBuilder sb = new StringBuilder();
        sb.Capacity = 256;
        USER32.GetClassName(wParam, sb, 256);
        className = sb.ToString();
        IsDialog = (className == "#32770");
      }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////
    #region Class CbtHook

    /// <summary>
    /// Class to expose the windows WH_CBT hook mechanism.
    /// </summary>
    public class CbtHook : WindowsHook {
      /// <summary>
      /// WH_CBT hook delegate method.
      /// </summary>
      public delegate void CbtEventHandler(object sender, CbtEventArgs e);

      /// <summary>
      /// WH_CBT create event.
      /// </summary>
      public event CbtEventHandler WindowCreate;
      /// <summary>
      /// WH_CBT destroy event.
      /// </summary>
      public event CbtEventHandler WindowDestroye;
      /// <summary>
      /// WH_CBT activate event.
      /// </summary>
      public event CbtEventHandler WindowActivate;

      /// <summary>
      /// Construct a WH_CBT hook.
      /// </summary>
      public CbtHook()
        : base(HookType.WH_CBT) {
        this.HookInvoke += new HookEventHandler(CbtHookInvoked);
      }
      /// <summary>
      /// Construct a WH_CBT hook giving a hook filter delegate method.
      /// </summary>
      /// <param name="func">Hook filter event.</param>
      public CbtHook(HookProc func)
        : base(HookType.WH_CBT, func) {
        this.HookInvoke += new HookEventHandler(CbtHookInvoked);
      }

      // handles the hook event
      private void CbtHookInvoked(object sender, HookEventArgs e) {
        // handle hook events (only a few of available actions)
        switch ((CbtHookAction)e.code) {
          case CbtHookAction.HCBT_CREATEWND:
            HandleCreateWndEvent(e.wParam, e.lParam);
            break;
          case CbtHookAction.HCBT_DESTROYWND:
            HandleDestroyWndEvent(e.wParam, e.lParam);
            break;
          case CbtHookAction.HCBT_ACTIVATE:
            HandleActivateEvent(e.wParam, e.lParam);
            break;
        }
        return;
      }

      // handle the CREATEWND hook event
      private void HandleCreateWndEvent(IntPtr wParam, IntPtr lParam) {
        if (WindowCreate != null) {
          CbtEventArgs e = new CbtEventArgs(wParam, lParam);
          WindowCreate(this, e);
        }
      }

      // handle the DESTROYWND hook event
      private void HandleDestroyWndEvent(IntPtr wParam, IntPtr lParam) {
        if (WindowDestroye != null) {
          CbtEventArgs e = new CbtEventArgs(wParam, lParam);
          WindowDestroye(this, e);
        }
      }

      // handle the ACTIVATE hook event
      private void HandleActivateEvent(IntPtr wParam, IntPtr lParam) {
        if (WindowActivate != null) {
          CbtEventArgs e = new CbtEventArgs(wParam, lParam);
          WindowActivate(this, e);
        }
      }
    }
    #endregion
}
