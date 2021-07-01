
namespace GOH_WaterDragon_Loop
{
    partial class mainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btn_startThread = new System.Windows.Forms.Button();
            this.tb_Log = new System.Windows.Forms.TextBox();
            this.btn_stopThread = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(174, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 100);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // btn_startThread
            // 
            this.btn_startThread.Location = new System.Drawing.Point(12, 12);
            this.btn_startThread.Name = "btn_startThread";
            this.btn_startThread.Size = new System.Drawing.Size(75, 23);
            this.btn_startThread.TabIndex = 1;
            this.btn_startThread.Text = "시작";
            this.btn_startThread.UseVisualStyleBackColor = true;
            this.btn_startThread.Click += new System.EventHandler(this.btn_searchWater_Click);
            // 
            // tb_Log
            // 
            this.tb_Log.Location = new System.Drawing.Point(12, 41);
            this.tb_Log.Multiline = true;
            this.tb_Log.Name = "tb_Log";
            this.tb_Log.ReadOnly = true;
            this.tb_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_Log.Size = new System.Drawing.Size(156, 286);
            this.tb_Log.TabIndex = 3;
            // 
            // btn_stopThread
            // 
            this.btn_stopThread.Location = new System.Drawing.Point(93, 12);
            this.btn_stopThread.Name = "btn_stopThread";
            this.btn_stopThread.Size = new System.Drawing.Size(75, 23);
            this.btn_stopThread.TabIndex = 4;
            this.btn_stopThread.Text = "중지";
            this.btn_stopThread.UseVisualStyleBackColor = true;
            this.btn_stopThread.Click += new System.EventHandler(this.btn_stopThread_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(731, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "시작";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(818, 339);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_stopThread);
            this.Controls.Add(this.tb_Log);
            this.Controls.Add(this.btn_startThread);
            this.Controls.Add(this.pictureBox1);
            this.Name = "mainForm";
            this.Text = "상태: 중지중";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.mainForm_FormClosed);
            this.Load += new System.EventHandler(this.mainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btn_startThread;
        private System.Windows.Forms.TextBox tb_Log;
        private System.Windows.Forms.Button btn_stopThread;
        private System.Windows.Forms.Button button1;
    }
}

