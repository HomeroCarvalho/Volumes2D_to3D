using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
// EM PORTUGUÊS:
//  ESTE PROGRAMA COMEÇOU EM 1999, MAS SÓ AGORA PUDE TERMINÁ-LO...
//  O PROGRAMA PARTE DO PRESSUPOSTO QUE O ÂNGULO DE INCLINAÇÃO DE UM GRID (X,Y) É PROPORCIONAL À ÁREA DE ILUMINAÇÃO.
//  MAS A ÁREA DE ILUMINAÇÃO É PROPORCIONAL (PODE SER PROPORCIONAL ^2) AO BRILHO DAS CORES HSV (BRILHO= COMPONENTE
//  VALUE DO PADRÃO HSV).ENTÃO, HÁ UMA INTERPOLAÇÃO QUE RELACIONA O BRILHO DA COR CONVERTIDA NO PADRÃO HSV COM O 
//  ÂNGULO DE INCLINAÇÃO. SE A COR MUDA DE UM BRILHO MENOR PARA UM BRILHO MAIOR, ACRESCENTA-SE 90 GRAUS, POIS A ÁREA
//  DE ILUMINAÇÃO CURRENTE É MAIOR QUE A ÁREA DE ILUMINAÇÃO ANTERIOR, E O ÂNGULO QUE SE CALCULA NA INTERPOLAÇÃO ESTAVA
//  ENTRE 0 A 90 GRAUS.
//  INFORMAÇÃO SOBRE O PROGRAMA: NÃO SE PREOCUPE SE A IMAGEM FINAL ULTRAPASSAR OS LIMITES DA TELA, AO SALVAR A IMAGEM
//  É INCLUÍDO A PARTE QUE ESTÁ ALÉM DA TELA.

// IN ENGLISH:
// THIS BEGAN PROGRAM IN 1999, BUT ONLY NOW MIGHT FINISH IT ...
// THE PROGRAM OF THE ASSUMPTION THAT PART TILT ANGLE OF A GRID (X, Y) is PROPORTIONAL TO LIGHTING AREA.
// BUT LIGHTING AREA IS PROPORTIONAL (BE PROPORTIONAL ^ 2) THE BRIGHT COLORS HSV (SHINE = COMPONENT
// VALUE STANDARD HSV) .Then, THERE IS A INTERPOLATION RELATES TO SHINE COLOR CONVERTED IN THE STANDARD WITH HSV
// TILT ANGLE. IF THE COLOR CHANGING OF A BRIGHT SHINING A SMALLER BIGGER, ADDS UP 90 DEGREES OR THE AREA
// OF CURRENT LIGHTING IS BIGGER THAN PREVIOUS LIGHTING AREA, AND ANGLE IN CALCULATING THAT WAS INTERPOLATION
// BETWEEN 0 A 90 DEGREES.
// INFORMATION ABOUT THE PROGRAM: DO NOT WORRY IF THE IMAGE EXCEED THE LIMITS FINAL SCREEN, THE SAVE IMAGE
// IS INCLUDED THE PARTY IS BEYOND THE SCREEN.
namespace wndHSVcolorsParaVolume
{
    public partial class wndWindow : Form
    {
        /// <summary>
        /// singleton da classe wndWindow.
        /// </summary>
        private static wndWindow sngltWindow = null;
        /// <summary>
        /// raio mínimo para o cálculo da profundidade (fator multiplicativo para a coordenada z).
        /// </summary>
        private float raioMinimo = 40.0F;
        /// <summary>
        /// fator multiplicativo para cada ponto (x,y) da imagem 3D.
        /// </summary>
        private int deltaX = 15;
        /// <summary>
        /// cena final, calculada as profundidades.
        /// </summary>
        private Bitmap CenaFinal;

        private enum typeFormat {grid, fill }

        private typeFormat typeFormatDrawing = typeFormat.grid;

        /// <summary>
        /// singleton para a classe [wndWindow].
        /// </summary>
        /// <returns>retorna a única instância de [wndWindow].</returns>
        public static wndWindow getInstance()
        {
            if (sngltWindow == null)
                sngltWindow = new wndWindow();
            return sngltWindow;
        } // getInstance()

        /// <summary>
        /// construtor de acesso privado, para cálculo do singleton [wndWindow].
        /// </summary>
        private wndWindow()
        {
            Utils.Log log = new Utils.Log(Path.GetFullPath("log.txt"));
            InitializeComponent();
        } // wndWindow()
        /// <summary>
        /// funciona como um arquivo de log.
        /// </summary>
        /// <param name="msg">mensagem a ser gravada.</param>
        public void Mensagem(string msg)
        {
            this.cmbErros.Items.Add(DateTime.Now.ToLongTimeString() + ": " + msg.ToString());
            cmbErros.SelectedIndex = this.cmbErros.Items.Count - 1;
            Utils.Log.addMessage(DateTime.Now.ToLongTimeString() + "-->" + msg.ToString());
        }// Mensagem()
        /// <summary>
        /// calcula o ângulo em radianos segundo o brilho (fator value do padrão HSV),
        /// a partir de uma cor RGB.
        /// </summary>
        /// <param name="c">Cor hsv a calcular</param>
        /// <returns>retorna um ângulo por interpolação linear.</returns>
        private float calcAnguloInclinacao(Color c)
        {
            corHSV.conversorRGBtoHSV conversor = new corHSV.conversorRGBtoHSV();
            corHSV.ColorHSV corhsv = conversor.UpdateHSV(c);
            // calcula a interpolação entre o brilho da cor HSV e o ângulo de inclinação.
            double angulo = (corhsv.v / 255) * Math.PI/2.0;
            return ((float)Math.Acos(angulo));

        } // calcAnguloInclinacao()
        /// <summary>
        /// calcula o ângulo em radianos segundo o brilho (fator value do padrão HSV),
        /// a partir de uma cor HSV.
        /// </summary>
        /// <param name="c1">cor HSV para cálculo.</param>
        /// <returns>retorna um ângulo por interpolação linear.</returns>
        private float calcAnguloInclinacao(corHSV.ColorHSV c1)
        {
            float angulo = (float)((c1.v / 255.0F) * Math.PI/2.0);
         
            return ((float)Math.Acos(angulo));

        } // calcAnguloInclinacao()
        /// <summary>
        /// calcula a profundidade da imagem de entrada,
        /// utilizando a posição e a inclinação por brilho
        /// (fator v das cores HSV).
        /// </summary>
        /// <param name="cena">Imagem utilizada.</param>
        /// <returns>retorna uma matriz bidimensional com as profundidades.</returns>
        private float[,] calcProfundidades(Bitmap cena)
        {
            float[,] profundidade = new float[cena.Width, cena.Height];
            corHSV.conversorRGBtoHSV conversor = new corHSV.conversorRGBtoHSV();
            for (int y = 0; y < (cena.Height - 1); y++)
                for (int x = 0; x < cena.Width; x++)
                {
                    if (cena.GetPixel(x, y).A < 10) continue;
                    if (cena.GetPixel(x, y + 1).A <10) continue;
                    corHSV.ColorHSV c1 = conversor.UpdateHSV(cena.GetPixel(x, y));
                    corHSV.ColorHSV c2 = conversor.UpdateHSV(cena.GetPixel(x, y + 1));
                    PointF p1 = new PointF(x, y);
                    float z = calcLadosInclinacao(p1, c1, c2);
                    profundidade[x, y] = z;
                } // for x
            return profundidade;
        }// calcVolume()
        /// <summary>
        /// calcula a inclinação a partir da cor de dois pontos, tomando como base o ponto [p1].
        /// </summary>
        /// <param name="p1">ponto base para o cálculo.</param>
        /// <param name="c1">cor HSV do ponto 1.</param>
        /// <param name="c2">cor HSV do ponto 2.</param>
        /// <returns>retorna a inclinação (coordenada z do ponto) do ponto p1.</returns>
        private float calcLadosInclinacao(PointF p1, corHSV.ColorHSV c1, corHSV.ColorHSV c2)
        {
            float angulo1 = this.calcAnguloInclinacao(c1);
            if (c2.v > c1.v)
            {
                angulo1 += (float)(Math.PI / 2);
            } // if c2.v>c1.v
            float z=(float)(this.raioMinimo * Math.Sin(angulo1));
            return z;
        } // calcLadosInclinacao()
        /// <summary>
        /// calcula um ponto segundo uma perspectiva isométrica.
        /// </summary>
        /// <param name="ponto2D">ponto 2D do quadriculado da imagem 2D.</param>
        /// <param name="z">profundidade calculada.</param>
        /// <returns>retorna um ponto 3D em perspectiva isométrica.</returns>
        private PointF calcPonto3D(PointF ponto2D, float z)
        {
            return new PointF(ponto2D.X + z / 2, ponto2D.Y + z / 2);
        } // calcPonto3D()

        /// <summary>
        /// Desenha a imagem 3D a partir do cálculo de profundidade segundo o padrão HSV.
        /// </summary>
        /// <param name="cena">Imagem 2D de entrada, a ter a profundidade ser calculada.</param>
        /// <param name="g">dispositivo gráfico para o desenho da imagem 3D.</param>
        /// <param name="origem">ponto a ser adicionado no gráfico da imagem 3D.</param>
        public void draw3DImage(Bitmap cena, Graphics g, Point origem)
        {
            float[,] profundidade = this.calcProfundidades(cena);
            PointF[,] pontos3D = new PointF[2, 2];
            List<PointF> lstPontos3D = new List<PointF>();

            for (int y = 0; y < cena.Height - 1; y++)
                for (int x = 0; x < cena.Width - 1; x++)
                {
                    // calcula os pontos 3D de um quadriculado polígono da imagem 3D.
                    pontos3D[0, 0] = this.calcPonto3D(new PointF(deltaX * x, deltaX * y), profundidade[x, y]);
                    pontos3D[1, 0] = this.calcPonto3D(new PointF(deltaX * (x + 1), deltaX * y), profundidade[x + 1, y]);
                    pontos3D[0, 1] = this.calcPonto3D(new PointF(deltaX * x, deltaX * (y + 1)), profundidade[x, y + 1]);
                    pontos3D[1, 1] = this.calcPonto3D(new PointF(deltaX * (x + 1), deltaX * (y + 1)), profundidade[x + 1, y + 1]);
                    // soma a origem, e faz cálculos de clipping (coordenadas <0).
                    this.somaOrigem(ref pontos3D[0, 0], origem);
                    this.somaOrigem(ref pontos3D[0, 1], origem);
                    this.somaOrigem(ref pontos3D[1, 1], origem);
                    this.somaOrigem(ref pontos3D[1, 0], origem);
                    // calcula a lista de pontos a serem desenhados como um polígono quadrilátero.
                    lstPontos3D.Add(pontos3D[0, 0]);
                    lstPontos3D.Add(pontos3D[1, 0]);
                    lstPontos3D.Add(pontos3D[1, 1]);
                    lstPontos3D.Add(pontos3D[0, 1]);
                    lstPontos3D.Add(pontos3D[0, 0]);
                    if (typeFormatDrawing == typeFormat.fill)
                        // desenha um imagem completamente preenchida 3D, no formato fill.
                        g.FillPolygon(new SolidBrush(cena.GetPixel(x, y)), lstPontos3D.ToArray());
                    if (typeFormatDrawing == typeFormat.grid)
                        // desenha um quadriculado da imagem 3D, no format grid.
                        g.DrawPolygon(new Pen(cena.GetPixel(x, y)), lstPontos3D.ToArray());
                    // limpa a lista de pontos.
                    lstPontos3D.Clear();
                } // for x
        } // draw3DImage()

        /// <summary>
        /// soma a origem ao ponto 2D, e valida e faz o corte no ponto 2D.
        /// </summary>
        /// <param name="p">ponto a ser calculado.</param>
        /// <param name="origem">coordenada da origem.</param>
        private void somaOrigem(ref PointF p, PointF origem)
        {
            p.X += origem.X;
            p.Y += origem.Y;
            this.clipping(ref p, origem);
        } // somaOrigem()
        
        /// <summary>
        /// valida os pontos. Corta as cordenadas <0.
        /// </summary>
        /// <param name="p">ponto a ser validado</param>
        private void clipping(ref PointF p, PointF origem)
        {
            if (p.X < 0)
                p.X = 0;
            if (p.Y < 0)
                p.Y = 0;
        }// validaPontos()

        /// <summary>
        /// sai do programa, não antes de perguntar se quer sair mesmo.
        /// </summary>
        /// <param name="sender">parâmetro de evento.</param>
        /// <param name="e">parâmetro de evento.</param>
        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Tem certeza que quer sair?", "Saída do Software ", MessageBoxButtons.YesNo) == DialogResult.Yes)
                System.Environment.Exit(0);

        }// sairToolStripMenuItem_Click

        /// <summary>
        /// abre um arquivo de imagem para trabalhar.
        /// </summary>
        /// <param name="sender">parâmetro de evento.</param>
        /// <param name="e">parâmetro de evento.</param>
        private void abrirImagemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnFlDlg = new OpenFileDialog();
            opnFlDlg.Filter = "PNG FORMAT|*.png|JPEG FORMAT|*.jpg|GIF FORMAT|*.GIF|BMP FORMTAT|*.BMP";
            if (opnFlDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap Cena = new Bitmap(System.IO.Path.GetFullPath(opnFlDlg.FileName));
                    this.CenaFinal = new Bitmap(Cena.Width * deltaX, Cena.Height * deltaX);

                    // calcula e limpa o dispositivo gráfico associado à imagem de cena resultante.
                    Graphics g = Graphics.FromImage(this.CenaFinal);

                    // faz o cálculo da imagem 3D.
                    this.draw3DImage(Cena, g, new Point(20, 20));

                    // desenha a imagem 3D calculada.
                    this.CreateGraphics().DrawImage(this.CenaFinal, new Point(0, 0));
                } // try
                catch( Exception ex)
                {
                    Mensagem("Error: try a smaller image. Software Msg Error: " + ex.Message);
                }
            } // if opnFlDlg.ShowDialog()
            else
            {
                Mensagem("Error in loading initial image");
            }  //else
        } // abriImagemToolStripMenuItem_Click()

        /// <summary>
        /// salva um arquivo de imagem.
        /// </summary>
        /// <param name="sender">parâmetro de evento.</param>
        /// <param name="e">parâmetro de evento</param>
        private void salvarImagemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog svFlDlg = new SaveFileDialog();
            svFlDlg.Filter = "PNG FORMAT|*.png|JPEG FORMAT|*.jpg|GIF FORMAT|*.gif|BITMAP FORMAT|*.bmp|all FILES|*.*";
            if (svFlDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this.CenaFinal.Save(Path.GetFullPath(svFlDlg.FileName));
                    Mensagem("Image successfully saved.");

                }// try
                catch (Exception ex)
                {
                    Mensagem("Error loading the processed image. System error msg.: " + ex.Message);
                } // catch
            } // if (svFlDlg.ShowDialog()==OK)
        } // void salvarImagemToolStripMenuItem_Click()
        
        /// <summary>
        /// muda o tipo de desenho para a imagem 3D final.
        /// </summary>
        /// <param name="sender">objeto do evento.</param>
        /// <param name="e">parâmetro do evento.</param>
        private void cmbBxFormatChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBxFormatChoice.SelectedIndex==0)
            {
                this.typeFormatDrawing = typeFormat.grid;
                this.Mensagem("You choice a grid format for 3d image drawing!");
            } // if
            if (cmbBxFormatChoice.SelectedIndex==1)
            {
                this.typeFormatDrawing = typeFormat.fill;
                this.Mensagem("You choice a fill format for 3d image drawing!");
            } //if
        } // cmbBxFormatChoice_SelectedIndexChanged()
    } // class wndWindow
} // namespace
