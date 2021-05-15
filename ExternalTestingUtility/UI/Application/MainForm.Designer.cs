
namespace ExternalTestingUtility
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.InnerForm = new Refract.UI.Core.Controls.CBorderedForm();
            this.ExampleRPC4 = new System.Windows.Forms.Button();
            this.RPCExample3 = new System.Windows.Forms.Button();
            this.RPCExample2 = new System.Windows.Forms.Button();
            this.RPCTest1 = new System.Windows.Forms.Button();
            this.ExampleRPC5 = new System.Windows.Forms.Button();
            this.InnerForm.ControlContents.SuspendLayout();
            this.SuspendLayout();
            // 
            // InnerForm
            // 
            this.InnerForm.BackColor = System.Drawing.Color.DodgerBlue;
            // 
            // InnerForm.ControlContents
            // 
            this.InnerForm.ControlContents.Controls.Add(this.ExampleRPC5);
            this.InnerForm.ControlContents.Controls.Add(this.ExampleRPC4);
            this.InnerForm.ControlContents.Controls.Add(this.RPCExample3);
            this.InnerForm.ControlContents.Controls.Add(this.RPCExample2);
            this.InnerForm.ControlContents.Controls.Add(this.RPCTest1);
            this.InnerForm.ControlContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InnerForm.ControlContents.Enabled = true;
            this.InnerForm.ControlContents.Location = new System.Drawing.Point(0, 32);
            this.InnerForm.ControlContents.Name = "ControlContents";
            this.InnerForm.ControlContents.Size = new System.Drawing.Size(796, 414);
            this.InnerForm.ControlContents.TabIndex = 1;
            this.InnerForm.ControlContents.Visible = true;
            this.InnerForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InnerForm.Location = new System.Drawing.Point(0, 0);
            this.InnerForm.Name = "InnerForm";
            this.InnerForm.Size = new System.Drawing.Size(800, 450);
            this.InnerForm.TabIndex = 0;
            this.InnerForm.TitleBarTitle = "External Cheat Example";
            this.InnerForm.UseTitleBar = true;
            // 
            // ExampleRPC4
            // 
            this.ExampleRPC4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExampleRPC4.Location = new System.Drawing.Point(8, 44);
            this.ExampleRPC4.Name = "ExampleRPC4";
            this.ExampleRPC4.Size = new System.Drawing.Size(80, 30);
            this.ExampleRPC4.TabIndex = 3;
            this.ExampleRPC4.Text = "MP Classes";
            this.ExampleRPC4.UseVisualStyleBackColor = true;
            this.ExampleRPC4.Click += new System.EventHandler(this.ExampleRPC4_Click);
            // 
            // RPCExample3
            // 
            this.RPCExample3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RPCExample3.Location = new System.Drawing.Point(180, 8);
            this.RPCExample3.Name = "RPCExample3";
            this.RPCExample3.Size = new System.Drawing.Size(80, 30);
            this.RPCExample3.TabIndex = 2;
            this.RPCExample3.Text = "Start Game";
            this.RPCExample3.UseVisualStyleBackColor = true;
            this.RPCExample3.Click += new System.EventHandler(this.RPCExample3_Click);
            // 
            // RPCExample2
            // 
            this.RPCExample2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RPCExample2.Location = new System.Drawing.Point(94, 8);
            this.RPCExample2.Name = "RPCExample2";
            this.RPCExample2.Size = new System.Drawing.Size(80, 30);
            this.RPCExample2.TabIndex = 1;
            this.RPCExample2.Text = "Host Dvars";
            this.RPCExample2.UseVisualStyleBackColor = true;
            this.RPCExample2.Click += new System.EventHandler(this.RPCExample2_Click);
            // 
            // RPCTest1
            // 
            this.RPCTest1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RPCTest1.Location = new System.Drawing.Point(8, 8);
            this.RPCTest1.Name = "RPCTest1";
            this.RPCTest1.Size = new System.Drawing.Size(80, 30);
            this.RPCTest1.TabIndex = 0;
            this.RPCTest1.Text = "Disconnect";
            this.RPCTest1.UseVisualStyleBackColor = true;
            this.RPCTest1.Click += new System.EventHandler(this.RPCTest1_Click);
            // 
            // ExampleRPC5
            // 
            this.ExampleRPC5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExampleRPC5.Location = new System.Drawing.Point(94, 44);
            this.ExampleRPC5.Name = "ExampleRPC5";
            this.ExampleRPC5.Size = new System.Drawing.Size(80, 30);
            this.ExampleRPC5.TabIndex = 4;
            this.ExampleRPC5.Text = "ZM Classes";
            this.ExampleRPC5.UseVisualStyleBackColor = true;
            this.ExampleRPC5.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.InnerForm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.Text = "External Testing";
            this.InnerForm.ControlContents.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Refract.UI.Core.Controls.CBorderedForm InnerForm;
        private System.Windows.Forms.Button RPCTest1;
        private System.Windows.Forms.Button RPCExample2;
        private System.Windows.Forms.Button RPCExample3;
        private System.Windows.Forms.Button ExampleRPC4;
        private System.Windows.Forms.Button ExampleRPC5;
    }
}

