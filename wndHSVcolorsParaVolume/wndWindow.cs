using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using Utils;
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
        private Bitmap Cena = null;
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

        private enum typePerspective { Isometric, Geometric, SuperIsometric, DimetricNEN_ISO, Dimetrica_Chinese_Scroll_Paints,
                                       Dimetric_ComputerGames_sideView, Dimetric_ComputerGames_topView, Isometric_Computer_Games }

        private typeFormat typeFormatDrawing = typeFormat.grid;

        private typePerspective typeFormatPerspective = typePerspective.Isometric;


        Matriz matrizProjecaoIsometricaAlfa = new Matriz(3, 3);

        Matriz matrizProjecaoIsometricaBeta = new Matriz(3, 3);

        Matriz matrizTransformacaoGama = new Matriz(3, 3);

        Matriz matrizProjecaoTotal = new Matriz(3, 3);
        /// <summary>
        /// delegate para cálculo de um tipo de perspectiva.
        /// </summary>
        /// <param name="v">ponto 3D a ser convertido em 2D, segundo os cálculos da perspectiva.</param>
        /// <returns>retorna um ponto 2D resultante do cálculo de perspectiva aplicado no ponto 3D de entrada.</returns>
        public delegate vetor2 Type_perspective(vetor3 v);
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
            double alfa = Math.Atan(1.0 / Math.Sqrt(2.0));
            double beta = Utils.angulos.toRadianos(45);

            configuraMatrizProjecaoIsometrica(alfa, beta);

            matrizProjecaoTotal = matrizProjecaoIsometricaAlfa * matrizProjecaoIsometricaBeta;


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
        /// configura a matriz de projeção isométrica.
        /// </summary>
        /// <param name="alfa">ângulo alfa em radianos.</param>
        /// <param name="beta">ângulo beta em radianos.</param>
        private void configuraMatrizProjecaoIsometrica(double alfa, double beta)
        {
            matrizProjecaoIsometricaAlfa.setElemento(0, 0, 1.0);
            matrizProjecaoIsometricaAlfa.setElemento(0, 1, 0.0);
            matrizProjecaoIsometricaAlfa.setElemento(0, 2, 0.0);
            matrizProjecaoIsometricaAlfa.setElemento(1, 0, 0.0);
            matrizProjecaoIsometricaAlfa.setElemento(1, 1,  Math.Cos(alfa));
            matrizProjecaoIsometricaAlfa.setElemento(1, 2, -Math.Sin(alfa));
            matrizProjecaoIsometricaAlfa.setElemento(2, 0, 0.0);
            matrizProjecaoIsometricaAlfa.setElemento(2, 1,  Math.Sin(alfa));
            matrizProjecaoIsometricaAlfa.setElemento(2, 2,  Math.Cos(alfa));

            matrizProjecaoIsometricaBeta.setElemento(0, 0,  Math.Cos(beta));
            matrizProjecaoIsometricaBeta.setElemento(0, 1, 0.0);
            matrizProjecaoIsometricaBeta.setElemento(0, 2,  Math.Sin(beta));
            matrizProjecaoIsometricaBeta.setElemento(1, 0, 0.0);
            matrizProjecaoIsometricaBeta.setElemento(1, 1, 1.0);
            matrizProjecaoIsometricaBeta.setElemento(1, 2, 0.0);
            matrizProjecaoIsometricaBeta.setElemento(2, 0, -Math.Sin(beta));
            matrizProjecaoIsometricaBeta.setElemento(2, 1, 0.0);
            matrizProjecaoIsometricaBeta.setElemento(2, 2,  Math.Cos(beta));

            matrizTransformacaoGama.setElemento(0, 0, 1.0);
            matrizTransformacaoGama.setElemento(0, 1, 0.0);
            matrizTransformacaoGama.setElemento(0, 2, 0.0);
            matrizTransformacaoGama.setElemento(1, 0, 0.0);
            matrizTransformacaoGama.setElemento(1, 1, 1.0);
            matrizTransformacaoGama.setElemento(1, 2, 0.0);
            matrizTransformacaoGama.setElemento(2, 0, 0.0);
            matrizTransformacaoGama.setElemento(2, 1, 0.0);
            matrizTransformacaoGama.setElemento(2, 2, 0.0);
        } // configuraMatrizProjecaoIsometrica()

        /// <summary>
        /// Faz a projeção isométrica com uma matriz de transformação.
        /// </summary>
        /// <param name="v">vetor 3D a ser transformado.</param>
        /// <returns>retorna um vetor 3D representando a projeção isométrica.</returns>
        private vetor2 calcPonto3D_IsometricaConfiguravel(vetor3 v)
        {

            Matriz a = Matriz.DeVetorParaMatriz(v);
            Matriz b = matrizTransformacaoGama * matrizProjecaoTotal * a;
            vetor3 v3f = Matriz.DeMatrizParaVetor(b);
            vetor2 v2f = new vetor2(v3f.X, v3f.Y);
            return v2f;
        } // isometricaConfiguravel()

        private vetor2 calcPerspectivaDimetricaNEN_ISO(vetor3 v)
        {
            vetor2 p = new vetor2(0.0, 0.0);
            p.X = v.X * Math.Cos(angulos.toRadianos(7.0)) + v.Z * Math.Cos(angulos.toRadianos(42.0)) / 2.0;
            p.Y = v.Y + v.Z * Math.Sin(angulos.toRadianos(42.0)) / 2.0 - v.X * Math.Sin(angulos.toRadianos(7.0));
            return p;
        } // calcPerspectivaDimetrica()

        private vetor2 calcPerspectivaDimetricaChineseScrollPaints(vetor3 v)
        {
            double n = 0.56;
            double T = angulos.toRadianos(35);
            vetor2 p = new vetor2(0.0, 0.0);
            p.X = v.X + n * v.Z * Math.Cos(T);
            p.Y = v.Y + n * v.Z * Math.Sin(T);
            return p;
        } // calcPerspectivaDimetricaChineseScrollPaints()

        private vetor2 calcPonto3DPerspectivaDimetrica_sideView(vetor3 v)
        {
            vetor2 p = new vetor2(0.0, 0.0);
            p.X = v.X + v.Z / 2.0;
            p.Y = v.Y + v.Z / 4.0;
            return p;
        } // calcPonto3DPerspectivaDimetrica_sideView()


        private vetor2 calcPonto3DPerspectivaDimetricaComputerGames_topView(vetor3 v)
        {
            vetor2 p = new vetor2(0.0, 0.0);
            p.X = v.X + v.Z / 4.0;
            p.Y = v.Y + v.Z / 2.0;
            return p;
        } // calcPonto3DPerspectivaDimetricaComputerGames_topView()


        private vetor2 calcPonto3DPerspectivaIsometrica_ComputerGames(vetor3 v)
        {
            vetor2 p = new vetor2(0.0, 0.0);
            p.X = v.X - v.Z;
            p.Y = v.Y + (v.X + v.Z) / 2.0;

            return p;
        } //calcPonto3DPerspectivaIsometrica_ComputerGames()


        /// <summary>
        /// calcula um ponto segundo uma perspectiva isométrica.
        /// </summary>
        /// <param name="v">ponto 3D a ser convertido em ponto de tela 2D.</param>
        /// <returns>retorna um ponto 2D resultante da perspectiva isométrica.</returns>
        private vetor2 calcPonto3D_PerspectivaIsometrica(vetor3 v)
        {
            return new vetor2(v.X + v.Z / 2.0, v.Y + v.Z / 2.0);
        } // calcPonto3D_PerspectivaIsometrica()

        /// <summary>
        /// calcula um ponto segundo uma perspectiva geométrica.
        /// </summary>
        /// <param name="ponto2D">ponto 2D do quadriculado da imagem 2D.</param>
        /// <param name="z">coordenada de profundidade.</param>
        /// <returns>retorna um ponto 2D resultante da perspectiva geométrica.</returns>
        private vetor2 calcPonto3D_PerspectivaGeometrica(vetor3 v)
        {
            if (v.Z != 0.0)
                return new vetor2(v.X / v.Z, v.Y / v.Z);
            else
                return new vetor2(0.0, 0.0);
        } // calcPonto3D_PerspectivaGeometrica()

    
        /// <summary>
        /// Desenha a imagem 3D a partir do cálculo de profundidade segundo o padrão HSV.
        /// </summary>
        /// <param name="cena">Imagem 2D de entrada, a ter a profundidade ser calculada.</param>
        /// <param name="g">dispositivo gráfico para o desenho da imagem 3D.</param>
        /// <param name="origem">ponto a ser adicionado no gráfico da imagem 3D.</param>
        public void calculaProjecaoEmImagem(Bitmap cena, Graphics g, Point origem, Type_perspective  calcPonto3D)
        {
            float[,] profundidade = this.calcProfundidades(cena);
            vetor3[,] pontos3D = new vetor3[2, 2];
            List<PointF> lstPontos3D = new List<PointF>();
            vetor3 PontoMinimo = new vetor3(100000.0, 100000.0,10000.0);
            vetor3 PontoMaximo = new vetor3(-100000.0, -100000.0,-100000.0);
            for (int y = 0; y < cena.Height - 1; y++) 
            {
                for (int x = 0; x < cena.Width - 1; x++) 
                {
                    // calcula a lista de pontos que comporão o polígono em (x,y), e verifica se é ponto máximo ou ponto mínimo.
                    calcPontos3D(calcPonto3D, profundidade, ref pontos3D, y, x);
                    // verifica se o polígono em (x,y) contém um ponto Máximo ou Mínimo.
                    this.achaMinimoMaximo(ref PontoMinimo, ref PontoMaximo, pontos3D[0, 0]);
                    this.achaMinimoMaximo(ref PontoMinimo, ref PontoMaximo, pontos3D[0, 1]);
                    this.achaMinimoMaximo(ref PontoMinimo, ref PontoMaximo, pontos3D[1, 1]);
                    this.achaMinimoMaximo(ref PontoMinimo, ref PontoMaximo, pontos3D[1, 0]);
                } // for x
            }  // for y

            for (int y = 0; y < cena.Height - 1; y++)
                for (int x = 0; x < cena.Width - 1; x++)
                {
                    // calcula a lista de pontos a serem desenhados como um polígono quadrilátero.
                    calcPontos3D(calcPonto3D, profundidade, ref pontos3D, y, x);
                    
                    // calcula os pontos da Tela segundo o tipo de perspectiva.
                    vetor2 ponto2DTela00 = calcPonto3D(pontos3D[0, 0]);
                    vetor2 ponto2DTela10 = calcPonto3D(pontos3D[1, 0]);
                    vetor2 ponto2DTela11 = calcPonto3D(pontos3D[1, 1]);
                    vetor2 ponto2DTela01 = calcPonto3D(pontos3D[0, 1]);

                    ponto2DTela00 = 0.25 * ponto2DTela00;
                    ponto2DTela01 = 0.25 * ponto2DTela01;
                    ponto2DTela10 = 0.25 * ponto2DTela10;
                    ponto2DTela11 = 0.25 * ponto2DTela11;

                    // prepara os pontos da tela para a lista de PointF utilizados
                    // no desenho de polígono no dispositivo gráfico.
                    lstPontos3D.Add(new PointF((float)ponto2DTela00.X, (float)ponto2DTela00.Y));
                    lstPontos3D.Add(new PointF((float)ponto2DTela10.X, (float)ponto2DTela10.Y));
                    lstPontos3D.Add(new PointF((float)ponto2DTela11.X, (float)ponto2DTela11.Y));
                    lstPontos3D.Add(new PointF((float)ponto2DTela01.X, (float)ponto2DTela01.Y));
                    lstPontos3D.Add(new PointF((float)ponto2DTela00.X, (float)ponto2DTela00.Y));

                    if (typeFormatDrawing == typeFormat.fill)
                        // desenha um imagem completamente preenchida 3D, no formato fill.
                        g.FillPolygon(new SolidBrush(cena.GetPixel(x, y)), lstPontos3D.ToArray());
                    if (typeFormatDrawing == typeFormat.grid)
                        // desenha um quadriculado da imagem 3D, no format grid.
                        g.DrawPolygon(new Pen(cena.GetPixel(x, y)), lstPontos3D.ToArray());
                    // limpa a lista de pontos.
                    lstPontos3D.Clear();
                } // for x
            } // calculaProjecaoEmImagem()

        /// <summary>
        /// verifica se o ponto [ponto] contém coordenadas mínimas ou máximas. Se conter,
        /// retorna o mínimo X ou Y em [pontoMinimo], e retorna o máximo X ou Y em 
        /// [pontoMaximo].
        /// </summary>
        /// <param name="pontoMinimo">ponto Mínimo da solução.</param>
        /// <param name="pontoMaximo">ponto Máximo da solução.</param>
        /// <param name="ponto">ponto a ser verificado se contém coordenadas mínimas ou máximas.</param>
        private void achaMinimoMaximo(ref vetor3 pontoMinimo, ref vetor3  pontoMaximo, vetor3 ponto)
        {
            if (pontoMinimo.X > ponto.X)
                pontoMinimo.X = ponto.X;
            if (pontoMaximo.X < ponto.X)
                pontoMaximo.X = ponto.X;

            if (pontoMinimo.Y > ponto.Y)
                pontoMinimo.Y = ponto.Y;
            if (pontoMaximo.Y < ponto.Y)
                pontoMaximo.Y = ponto.Y;

            if (pontoMinimo.Z > ponto.Z)
                pontoMinimo.Z = ponto.Z;
            if (pontoMaximo.Z < ponto.Z)
                pontoMaximo.Z = ponto.Z;
        } //achaMinimoMaximo()

        /// <summary>
        /// calcula os pontos 2D de tela que formarão o quadrilátero em (x,y).
        /// </summary>
        /// <param name="calcPonto3D"></param>
        /// <param name="profundidade"></param>
        /// <param name="pontos3D"></param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        private void calcPontos3D(
            Type_perspective calcPonto3D, 
            float[,] profundidade, 
            ref vetor3[,] pontos3D,
            int y, int x)
        {
            // calcula os pontos 3D de um quadriculado polígono da imagem 3D.
            pontos3D[0, 0] = new vetor3(deltaX * x, deltaX * y, profundidade[x, y]);
            pontos3D[1, 0] = new vetor3(deltaX * (x + 1), deltaX * y, profundidade[x + 1, y]);
            pontos3D[0, 1] = new vetor3(deltaX * x, deltaX * (y + 1), profundidade[x, y + 1]);
            pontos3D[1, 1] = new vetor3(deltaX * (x + 1), deltaX * (y + 1), profundidade[x + 1, y + 1]);
           
        } // calcPontos3D()


        /// <summary>
        /// calcula a imagem com a transformação de perspectiva e cálculo de profundidade.
        /// </summary>
        /// <param name="Cena">Imagem a ser transformada.</param>
        private void calcImage(Bitmap Cena)
        {
            calcImageSecondPerspectiveChoiced(Cena, CenaFinal);
        } // DrawImage()

        private void calcImageSecondPerspectiveChoiced(Bitmap Cena, Bitmap CenaFinal)
        {
            if (this.Cena != null)
            {
                // inicializa a imagem que será renderizada pelo programa.
                CenaFinal = new Bitmap(Cena.Width * deltaX, Cena.Height * deltaX);
                // calcula e limpa o dispositivo gráfico associado à imagem de cena resultante.
                Graphics g = Graphics.FromImage(CenaFinal);
                // limpa o dispositivo gráfico da imagem resultante.
                g.Clear(Color.Black);
                // calcula a projeção segundo o tipo de formato escolhido.
                switch (this.typeFormatPerspective)
                {
                    case typePerspective.Isometric:
                        // cálculo com perspectiva isométrica.
                        this.calculaProjecaoEmImagem(Cena, g, new Point(20, 20), this.calcPonto3D_PerspectivaIsometrica);
                        break;
                    case typePerspective.Geometric:
                        // cálculo com perspectiva geométrica.
                        this.calculaProjecaoEmImagem(Cena, g, new Point(20, 20), this.calcPonto3D_PerspectivaGeometrica);
                        break;
                    case typePerspective.SuperIsometric:
                        // cálculo com perspectiva isométrica segundo a Wikipedia.
                        this.calculaProjecaoEmImagem(Cena, g, new Point(20, 20), this.calcPonto3D_IsometricaConfiguravel);
                        break;
                    case typePerspective.DimetricNEN_ISO:
                        // cálculo com a perspectiva dimétrica NEN ISO.
                        this.calculaProjecaoEmImagem(Cena, g, new Point(20, 20), this.calcPerspectivaDimetricaNEN_ISO);
                        break;
                    case typePerspective.Dimetrica_Chinese_Scroll_Paints:
                        // cálculo com a perspectiva dimétrica Chinese Scroll Paints.
                        this.calculaProjecaoEmImagem(Cena, g, new Point(20, 20), this.calcPerspectivaDimetricaChineseScrollPaints);
                        break;
                    case typePerspective.Dimetric_ComputerGames_sideView:
                        // cálculo com a perspectiva dimétrica computer games side view.
                        this.calculaProjecaoEmImagem(Cena, g, new Point(20, 20), this.calcPonto3DPerspectivaDimetrica_sideView);
                        break;
                    case typePerspective.Dimetric_ComputerGames_topView:
                        // cálculo com a perspectiva dimétrica computer games top view.
                        this.calculaProjecaoEmImagem(Cena, g, new Point(20, 20), this.calcPonto3DPerspectivaDimetricaComputerGames_topView);
                        break;
                    case typePerspective.Isometric_Computer_Games:
                        // cálculo com a perspectiva isométrica para computer games.
                        this.calculaProjecaoEmImagem(Cena, g, new Point(20, 20), this.calcPonto3DPerspectivaIsometrica_ComputerGames);
                        break;
                } // switch
            } // if Cena<>null
        }

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
            if (cmbBxFormatChoice.SelectedIndex == 0)
            {
                this.typeFormatDrawing = typeFormat.grid;
                this.Mensagem("You choice a grid format for 3d image drawing!");
            } // if
            if (cmbBxFormatChoice.SelectedIndex == 1)
            {
                this.typeFormatDrawing = typeFormat.fill;
                this.Mensagem("You choice a fill format for 3d image drawing!");
            } // if
            if (this.Cena != null)
            {
                Graphics g = this.CreateGraphics();
                g.Clear(Color.Black);
                this.calcImageSecondPerspectiveChoiced(this.Cena, this.CenaFinal);
            } // if Cena<>null
        } // cmbBxFormatChoice_SelectedIndexChanged()

        /// <summary>
        /// seleciona o tipo de perspectiva a ser aplicado.
        /// </summary>
        /// <param name="sender">objeto do evento.</param>
        /// <param name="e">parâmetros do evento.</param>
        private void cmbBxTypePerspective_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBxTypePerspective.SelectedIndex == 0)
            {
                this.typeFormatPerspective = typePerspective.Isometric;
            } // if
            if (cmbBxTypePerspective.SelectedIndex == 1)
            {
                this.typeFormatPerspective = typePerspective.Geometric;
            } // if
            if (cmbBxTypePerspective.SelectedIndex == 2)
            {
                this.typeFormatPerspective = typePerspective.SuperIsometric;
            } // if
            if (cmbBxTypePerspective.SelectedIndex == 3)
            {
                this.typeFormatPerspective = typePerspective.DimetricNEN_ISO;
            } // if
            if (cmbBxTypePerspective.SelectedIndex == 4)
            {
                this.typeFormatPerspective = typePerspective.Dimetrica_Chinese_Scroll_Paints;

            } // if
            if (cmbBxTypePerspective.SelectedIndex == 5)
            {
                this.typeFormatPerspective = typePerspective.Dimetric_ComputerGames_sideView;
            }
            if (cmbBxTypePerspective.SelectedIndex == 6)
            {
                this.typeFormatPerspective = typePerspective.Dimetric_ComputerGames_topView;
            }
            if (cmbBxTypePerspective.SelectedIndex==7)
            {
                this.typeFormatPerspective = typePerspective.Isometric_Computer_Games;
            }

            if (Cena != null)
            {
                this.calcImage(this.Cena);
                Graphics g = this.CreateGraphics();
                g.Clear(Color.Black);
                g.DrawImage(this.CenaFinal, new PointF(20.0F, 20.0F));
            } // if Cena<>null

        } // cmbBxTypePerspective_SelectedIndexChanged()
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
                    Cena = new Bitmap(System.IO.Path.GetFullPath(opnFlDlg.FileName));
                    calcImage(Cena);
                    Graphics g = this.CreateGraphics();
                    g.Clear(Color.Black);
                    // desenha a imagem 3D calculada.
                    g.DrawImage(this.CenaFinal, new Point(0, 0));
                } // try
                catch (Exception ex)
                {
                    Mensagem("Error: try a smaller image. Software Msg Error: " + ex.Message);
                }
            } // if opnFlDlg.ShowDialog()
            else
            {
                Mensagem("Error in loading initial image");
            }  //else
        } // abriImagemToolStripMenuItem_Click()
    } // class wndWindow
} // namespace
