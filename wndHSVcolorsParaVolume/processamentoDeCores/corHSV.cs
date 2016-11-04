using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;


namespace corHSV
{
    /// <summary>
    /// VETOR DE CORDENADAS ESPACIAIS DE NUMEROS INTEIROS, COM COMPONENTE COR.
    /// </summary>
    public class posicaoECor
    {
        public int x { get; set; }
        public int y { get; set; }
        public Color cor { get; set; }
        public posicaoECor(int xx, int yy, Color ccor)
        {
            this.x = xx;
            this.y = yy;
            this.cor = ccor;
        }
        public posicaoECor(posicaoECor v)
        {
            this.x = v.x;
            this.y = v.y;
            this.cor = v.cor;
        }
    }
    //************************************************************************************
    /// <summary>
    /// FAZ A CONVERSAO DE COR PARA COR GRAY 08 BITS, E OUTRAS UTILIDADES.
    /// </summary>
    public class cores_GRAY
    {
        public static int CORES_GRAY = 65536;

        /// <summary>
        /// gera uma cor na escala gray de 16 bits com base em [X,Y,Y].
        /// </summary>
        /// <param name="X">componente para [R,G,B] da nova cor.</param>
        /// <returns></returns>
        public static Color ToCorGray(int X, int Y)
        {
            return (Color.FromArgb(X, Y, Y));
        }

        /// <summary>
        /// devolve a cor gray de 8 bits da cor parametro.
        /// </summary>
        /// <param name="cor"></param>
        /// <returns></returns>
        public static Color geraCorEscalaGray(Color cor)
        {
            int componenteGray1 = cor.B;
            int componenteGray2 = (cor.G + cor.R) / 2;
            Color corGray = Color.FromArgb(componenteGray1,
                                           componenteGray2, componenteGray2);
            return (corGray);

        }
        /// <summary>
        /// gera o Bitmap bmpOut como uma cópia na escala gray de cores do Bitmap bmpIn.
        /// </summary>
        /// <param name="bmpIn">Bitmap imagem de entrada.</param>
        /// <param name="bmpOut">Bitmap imagem em escala gray de 08 bits.</param>
        public static void transformaBitmapParaGrayScale(Bitmap bmpIn, ref Bitmap bmpOut)
        {

            bmpOut = null;
            bmpOut = new Bitmap(bmpIn.Width, bmpIn.Height);
            int x, y;
            Color cor;
            for (y = 0; y < bmpIn.Height; y++)
                for (x = 0; x < bmpIn.Width; x++)
                {
                    cor = cores_GRAY.geraCorEscalaGray(bmpIn.GetPixel(x, y));
                    bmpOut.SetPixel(x, y, cor);
                } // for y
            angulos.angulosIngrimidade = new double[CORES_GRAY];
            for (x = 0; x < CORES_GRAY; x++)
                angulos.angulosIngrimidade[x] = ((double)(x)) * ((double)(Math.PI / 2)) / CORES_GRAY;


        } // public void transformaBitmapParaGrayScale
    } // class cores_GRAY
    //****************************************************************************************
    /// <summary>
    /// ESTA CLASSE CALCULA OS ANGULOS DE INCLINACAO EM RELACAO A CORES GRAY ESCALA DE 08 BITS.
    /// </summary>
    public class angulos
    {
        /// <summary>
        /// guarda os angulos de inclinação
        /// </summary>
        public static double[] angulosIngrimidade { set; get; }


        /// <summary>
        /// transforma uma cor em angulo em graus, utilizando uma escala gray de 08 bits.
        /// </summary>
        /// <param name="anguloIngrimidadeGray"></param>
        /// <returns></returns>
        public static double calculaAngulo(Color cor)
        {
            double anguloIngrimidadeGray = cores_GRAY.geraCorEscalaGray(cor).R;
            double a = anguloIngrimidadeGray * 90.00 / cores_GRAY.CORES_GRAY;

            return (a);

        }

        /// <summary>
        /// calcula fronteiras de objetos utilizando a lei de lambert, adaptada para imagens em escala gray 
        /// de 08 bits.
        /// </summary>
        /// <param name="bmpIn">Bitmap imagem de entrada.</param>
        /// <param name="bmpOut">Bitmap image com as fronteiras detectadas.</param>
        /// <param name="anguloGrausDiff">diferenca de angulos para ser considerado lado.</param>
        public void deteccaoDeLadosPelaLeiDeLambert(Bitmap bmpIn, ref Bitmap bmpOut,
            double anguloGrausDiff)
        {
            int x, y;
            bmpOut = new Bitmap(bmpIn.Width, bmpIn.Height);
            double ax, axPlus1, ayPlus1;
            for (y = 0; y < (bmpIn.Height - 1); y++)
                for (x = 0; x < (bmpIn.Width - 1); x++)
                {
                    ax = calculaAngulo(bmpIn.GetPixel(x, y));
                    axPlus1 = calculaAngulo(bmpIn.GetPixel(x + 1, y));
                    ayPlus1 = calculaAngulo(bmpIn.GetPixel(x, y + 1));
                    if ((Math.Abs(ax - axPlus1) > anguloGrausDiff)
                     || (Math.Abs(ax - ayPlus1) > anguloGrausDiff))
                    {
                        bmpOut.SetPixel(x, y, bmpIn.GetPixel(x, y));
                    }
                    else
                        bmpOut.SetPixel(x, y, Color.Black);
                } // for x
        } // void deteccaoDeLadosPelaLeiDeLambert()


    } // class angulos
    //*******************************************************************************************
    /// <summary>
    /// FAZ A CAPTURA DE IMAGENS POR CLUSTERS DE CORES.
    /// </summary>
    public class OBJETO_CLUSTERS
    {
        public List<Color> cores_presentes = null;
        public List<posicaoECor> cordenadasObjetoCluster = null;
        //_________________________________________________________________________________
        /// <summary>
        /// classe de cluster que guarda os dados de um conjunto de pontos, unidos pela cor.
        /// </summary>
        public class ClusterImagem
        {
            /// <summary>
            /// chave para o cluster. normalmente uma componente de cor gray de 08 bits [R,R,R].
            /// </summary>
            public int KEY_COLORX { get; set; }
            public int KEY_COLORY { get; set; }
            /// <summary>
            /// lista de pontos que obedecem a chave do cluster.
            /// </summary>
            public List<posicaoECor> cord2D = new List<posicaoECor>();
            /// <summary>
            /// inicializa um cluster, com a chave de cor especifica.
            /// </summary>
            /// <param name="keycolor"></param>
            public ClusterImagem(int keycolor1, int keycolor2)
            {
                this.KEY_COLORX = keycolor1;
                this.KEY_COLORY = keycolor2;

            }
        } // class ClusterImagem
        //___________________________________________________________________________________
        /// <summary>
        /// construtor basico. Apenas inicializa as variaveis, nao realiza nenhum servico utilitario.
        /// </summary>
        public OBJETO_CLUSTERS()
        {
            this.cores_presentes = new List<Color>();
            this.cordenadasObjetoCluster = new List<posicaoECor>();
        }

        /// <summary>
        /// construtor. aceita uma cor gray inicio e uma cor gray fim, e forma o cluster.O objeto e' formado
        /// por varios clusters.
        /// </summary>
        /// <param name="clusters">conjunto de imagens de clusters que serao unidos para formar um objeto.</param>
        /// <param name="inicio">cor gray inicio.</param>
        /// <param name="fim">cor gray fim.</param>
        public OBJETO_CLUSTERS(ClusterImagem[] clusters, int inicio, int fim)
        {
            this.cores_presentes = new List<Color>();
            this.cordenadasObjetoCluster = new List<posicaoECor>();

            int cluster;
            for (cluster = inicio; cluster < fim; cluster++)
                if (clusters[cluster] != null)
                {
                    // soma a lista de pontos do objeto cluster o ponto que nao estiver na lista.
                    this.cores_presentes.Add(
                        cores_GRAY.ToCorGray(clusters[cluster].KEY_COLORX,
                                             clusters[cluster].KEY_COLORY));

                    Color cor = this.cores_presentes[this.cores_presentes.Count - 1];
                    bool iguais = false;
                    int x, y;
                    for (x = 0; x < clusters[cluster].cord2D.Count; x++)
                    {
                        iguais = false;
                        for (y = 0; y < this.cores_presentes.Count; y++)
                            if (coresiguais(clusters[cluster].cord2D[x].cor, this.cores_presentes[y]) == true)
                            {
                                iguais = true;
                                break;
                            } // if clusters
                        if (iguais == false)
                            this.cordenadasObjetoCluster.Add(new posicaoECor(clusters[cluster].cord2D[x]));
                    }
                } // if clusters[cluster]!=null
        } // public OBJETO_CLUSTERS()

        private bool coresiguais(Color cor1, Color cor2)
        {
            return ((cor1.R == cor2.R) && (cor1.G == cor2.G) && (cor1.B == cor2.B));
        }
        /// <summary>
        /// calcula imagens atraves de pesquisa por "clusters". Cada cluster difere um do outro com
        /// uma fronteira cuja componente gray 8 bits da cor final do primeiro cluster, menos a com-
        /// ponente gray 8 bits da cor inicial do segundo cluster e' MAIOR em termos absolutos   que
        /// a variavel de entrada [MAX_angulo].
        /// </summary>
        /// <param name="bmpIn">imagem de entrada. contem a cena a ser processada.</param>
        /// <param name="bmpOut">conjunto de imagens geradas pelo metodo.</param>
        /// <param name="MAX_angulo">valor que se for maior entre uma
        ///                          cor e outra, adiciona um cluster
        ///                          e inicia um novo cluster.</param>
        public void calcClusters(Bitmap bmpIn, ref Bitmap[] bmpOut, double MAX_angulo)
        {
            /// PASSOS DO ALGORITMO:
            /// 1- INICIALIZAR AS VARIAVEIS E OBJETOS
            /// 2- NUMA MALHA, PARA CADA COMPONENTE [X] DE UMA COR GRAY DE 8 BITS CALCULADA A PARTIR
            /// DE UM SCANEAMENTO DA FIGURA-CENA, ASSOCIAR COMO CHAVE, E COLOCAR NA TABELA HASH:   A
            /// COR E AS CORDENADAS ESPACIAIS. PARA CADA ENTRADA  DA  TABELA  HASH,  HA'  UM  OBJETO
            /// CLUSTER ASSOCIADO.
            /// 3- COM A TABELA HASH POPULADA, CALCULA-SE OS OBJETOS CLUSTERS, VERIFICANDO AS ENTRA-
            /// DAS NULAS E AS ENTRADAS SEQUENCIAIS CUJA DIFERENCA QUE ULTRAPASSE [MAX_angulo]. NES-
            /// TE CASO, ADICIONA-SE UM CLUSTER, E INICIA UM NOVO, TENDO COMO COR INICIAL A  ULTIMA
            /// COR DA SEQUENCIA.
            /// 4- COM OS OBJETOS CLUSTERS POPULADOS, CALCULA-SE AS FRONTEIRAS DE CADA IMAGEM, E IN-
            /// SERE-SE OS DADOS DE CADA OBJETO CLUSTER EM UMA IMAGEM.


            // tabela hash para cores gray de 8 bits. Em vez de ordenar uma lista extensa de Cores Gray,
            // optou-se por utilizar uma tabela hash.
            ClusterImagem[] clusters = new ClusterImagem[cores_GRAY.CORES_GRAY + 1];

            // lista que contera os objetos clusters (um objeto cluster e' um conjunto
            // de clusters de cores gray dentro de um intervalo  que    obedecem     a
            // diferenca de [MAX_angulo] entre cores entre si.

            List<OBJETO_CLUSTERS> ObjetosClusters = new List<OBJETO_CLUSTERS>();

            // variaveis de proposito geral, usadas inicialmente para conter cordenadas espaciais.
            int x, y;

            // guarda o componente de 8 bits na escala Gray que forma uma cor. Usada com chave para a tabela
            // hash.
            int keyX = 0;
            int keyY = 0;
            for (y = 0; y < bmpIn.Height; y++)
                for (x = 0; x < bmpIn.Width; x++)
                {
                    // calcula a chave para a matriz hash, com base na componente da escala gray 08 bits.
                    keyX = cores_GRAY.geraCorEscalaGray(bmpIn.GetPixel(x, y)).R;
                    keyY = cores_GRAY.geraCorEscalaGray(bmpIn.GetPixel(x, y)).G;
                    // se nao ha ainda o cluster para esta chave, gera um objeto cluster
                    if (clusters[keyX * 256 + keyY] == null)
                        // cria um objeto cluster com base na componente de cor da escala gray de 08 bits.
                        clusters[keyX * 256 + keyY] = new ClusterImagem(keyX, keyY);
                    // soma as cordenadas e cor do ponto para o cluster.
                    clusters[keyX * 256 + keyY].cord2D.Add(new posicaoECor(x, y, bmpIn.GetPixel(x, y)));
                } // for x

            // inicio de um cluster de cores gray de 08 bits.
            int inicio = 0;
            // fim de um cluster de cores gray de 08 bits.
            int fim = 0;

            // valor para testar as fronteiras entre um cluster e outro.
            double ValorDiff = 0.0;


            // procura uma cor presente na matriz [cluster].
            while ((clusters[inicio] == null) && (inicio < (cores_GRAY.CORES_GRAY - 1)))
                inicio++;

            // cor Anterior aponta para a primeira cor presente, apontada por [inicio].
            Color corAnterior = cores_GRAY.ToCorGray(clusters[inicio].KEY_COLORX,
                clusters[inicio].KEY_COLORY);

            int absCORX = 0;
            int absCORY = 0;
            // inicia um loop para testar todas as cores da escala GRAY 16 bits.
            while (inicio < (cores_GRAY.CORES_GRAY - 1))
            {
                fim = inicio + 1;
                // pula as cores que nao estao presentes.
                while ((clusters[fim] == null) && (fim < (cores_GRAY.CORES_GRAY - 1)))
                    fim++;

                if (fim < (cores_GRAY.CORES_GRAY - 1))
                {
                    // modulo da diferenca da componente cor R     
                    absCORX = clusters[fim].KEY_COLORX - corAnterior.R;
                    if (absCORX < 0)
                        absCORX = -absCORX;
                    // modulo da diferenca da componente cor G
                    absCORY = clusters[fim].KEY_COLORY - corAnterior.G;
                    if (absCORY < 0)
                        absCORY = -absCORY;

                    // calcula a diferenca de angulos entre uma cor e outra.
                    ValorDiff = angulos.calculaAngulo(
                        cores_GRAY.ToCorGray(absCORX, absCORY));

                    // teste entre fronteiras de um cluster e outro.
                    if (100.00 * ValorDiff > MAX_angulo)
                    {
                        // se maior, inicia um novo objeto cluster, ou seja,
                        // soma um novo objeto com os cluster na faixa de
                        // cores dentro do intervalo estipulado.
                        ObjetosClusters.Add(new OBJETO_CLUSTERS(clusters, inicio, fim));
                    }
                    // cor anterior e' apontada para a ultima cor, ou aquela que e' apontada por [fim].
                    corAnterior = cores_GRAY.ToCorGray(clusters[fim].KEY_COLORX,
                                                       clusters[fim].KEY_COLORY);
                } // if (fim<CORES_GRAY-1)
                inicio = fim;
            } // while (inicio<CORES_GRAY-1)


            int objetos;

            // variaveis para as cordenadas extremas de um objeto cluster.
            int x1, y1, x2, y2;
            x1 = 0;
            x2 = 0;
            y1 = 0;
            y2 = 0;
            // inicializa a matriz de imagens, com o numero de objetos cluster.
            bmpOut = new Bitmap[ObjetosClusters.Count];
            for (objetos = 0; objetos < ObjetosClusters.Count; objetos++)
            {
                // detecta os extremos do objeto cluster, para inicializar a imagem
                // e retirar as cordenadas relativas (cordenadas do objeto  cluster 
                // menos as cordenadas do canto superior esquerdo superior).
                this.detectaFronteiras(ObjetosClusters[objetos].cordenadasObjetoCluster,
                    ref x1, ref y1, ref x2, ref y2);
                // inicia uma nova imagem para o objeto cluster.
                bmpOut[objetos] = new Bitmap(x2 - x1 + 1, y2 - y1 + 1);

                // popula a imagem com os dados do objeto cluster, retirando as cordenadas
                // do topo (canto esquerdo superior).
                for (x = 0; x < ObjetosClusters[objetos].cordenadasObjetoCluster.Count; x++)
                    bmpOut[objetos].SetPixel(
                        ObjetosClusters[objetos].cordenadasObjetoCluster[x].x - x1,
                        ObjetosClusters[objetos].cordenadasObjetoCluster[x].y - y1,
                        ObjetosClusters[objetos].cordenadasObjetoCluster[x].cor);
            } // for (objetos)

        } // void calcClusters()

        /// <summary>
        /// detecta as fronteiras (cordenadas maximas e minimas) de uma lista de cordenadas.
        /// </summary>
        /// <param name="cords">lista de pontos a ser pesquisada as cordenadas.</param>
        /// <param name="x1">parametro de retorno. Cordenada minima X.</param>
        /// <param name="y1">parametro de retorno. Cordenada maxima X.</param>
        /// <param name="x2">parametro de retorno. Cordenada minima Y.</param>
        /// <param name="y2">parametro de retorno. Cordenada maxima Y.</param>
        private void detectaFronteiras(List<posicaoECor> cords, ref int x1,
                                                        ref int y1, ref int x2, ref int y2)
        {
            x1 = +10000;
            y1 = +10000;
            x2 = -10000;
            y2 = -10000;
            int i;
            for (i = 0; i < cords.Count; i++)
            {
                if (cords[i].x < x1)
                    x1 = cords[i].x;
                if (cords[i].x > x2)
                    x2 = cords[i].x;
                if (cords[i].y < y1)
                    y1 = cords[i].y;
                if (cords[i].y > y2)
                    y2 = cords[i].y;
            } // for (i)
        } // void detectaFronteiras()

    } // class ClusterImagem
    //***************************************************************************************************
    /// <summary>
    /// CLASSE PARA PROCESSAMENTO DE SINAIS. CONTEM VARIOS METODOS CLASSICOS DE PROCESSAMENTO DE IMAGENS.
    /// </summary>
    public class BitmapProcessing
    {
        /// <summary>
        /// esta classe aplica uma matriz filter[,] , muito usado para processamento de imagens,
        /// num bitmap bmpIn, copiando o resultado para bmpOut.
        /// 
        /// </summary>
        /// <param name="filter">filtro de processamento de imagens.</param>
        /// <param name="bmpIn">Bitmap imagem de entrada.</param>
        /// <param name="bmpOut">Bitmap imagem gerado.</param>
        /// <param name="flag">tipo de processamento com o filtro.</param>
        public void aplicaConvolutionFilter(double[,] filter, int dimx, int dimy,
                                                       Bitmap bmpIn,
                                                   ref Bitmap bmpOut)
        {
            int x, y;
            int dx, dy;
            byte R, G, B;
            Color cor;
            int somaR = 0;
            int somaG = 0;
            int somaB = 0;
            bmpOut = new Bitmap(bmpIn.Width, bmpIn.Height);
            for (y = 0; y < bmpIn.Height - dimx; y++)
                for (x = 0; x < bmpIn.Width - dimy; x++)
                {
                    somaR = 0;
                    somaG = 0;
                    somaB = 0;
                    bmpOut.SetPixel(x, y, Color.Black);
                    for (dy = 0; dy < dimx; dy++)
                        for (dx = 0; dx < dimy; dx++)
                        {
                            cor = bmpIn.GetPixel(x + dx, y + dy);

                            R = cor.R;
                            G = cor.G;
                            B = cor.B;
                            somaR += (int)(R * filter[dx, dy]);
                            somaG += (int)(G * filter[dx, dy]);
                            somaB += (int)(B * filter[dx, dy]);
                        } // for dx

                    if ((somaR >= 0) && (somaG >= 0) && (somaB >= 0))
                    {
                        cor = Color.FromArgb(0, 0, 0);
                        int somaGray = somaR + somaG + somaB + 128;
                        if (somaGray > 255)
                            cor = Color.FromArgb(255, 255, 255);
                        else
                            //if (somaGray < 0)
                            //    cor = Color.FromArgb(0, 0, 0);
                            // else
                            cor = Color.FromArgb(somaGray, somaGray, somaGray);
                        bmpOut.SetPixel(x, y, cor);
                    } // if somaR


                } // for x             
        }//public static void aplicaConvolutionFilter


    }//public class BitmapProcessing
    //************************************************************************************************
    /// <summary>
    /// CLASSE PARA PROCESSAMENTO DE SINAIS. E' UM FILTRO DE SOBEL.
    /// </summary>
    public class sobel
    {
        Bitmap b;
        public Bitmap apply(Bitmap im)
        {
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };   //  The matrix Gx
            int[,] gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };  //  The matrix Gy
            b = im;
            Bitmap b1 = new Bitmap(im);
            for (int i = 1; i < b.Height - 1; i++)   // loop for the image pixels height
            {
                for (int j = 1; j < b.Width - 1; j++) // loop for image pixels width    
                {
                    float new_x = 0, new_y = 0;
                    float c;
                    for (int hw = -1; hw < 2; hw++)  //loop for cov matrix
                    {
                        for (int wi = -1; wi < 2; wi++)
                        {
                            c = (b.GetPixel(j + wi, i + hw).B + b.GetPixel(j + wi, i + hw).R + b.GetPixel(j + wi, i + hw).G) / 3;
                            new_x += gx[hw + 1, wi + 1] * c;
                            new_y += gy[hw + 1, wi + 1] * c;
                        }
                    }
                    if (new_x * new_x + new_y * new_y > 128 * 128)
                        b1.SetPixel(j, i, Color.Black);
                    else
                        b1.SetPixel(j, i, Color.White);
                }
            }
            return b1;
        } // apply()
    } // class sobel

    //***********************************************************************************************
    /// <summary>
    /// CLASSE PARA PROCESSAMENTO DE SINAIS. E' UM FILTRO DE LAPLACE.
    /// </summary>
    public class laplace
    {
        public double[,] filterLaplace = new double[,]{
                                                   {-1.0,-1.0,-1.0,-1.0,-1.0},
                                                   {-1.0,-1.0,-1.0,-1.0,-1.0},
                                                   {-1.0,-1.0,24.0,-1.0,-1.0},
                                                   {-1.0,-1.0,-1.0,-1.0,-1.0},
                                                   {-1.0,-1.0,-1.0,-1.0,-1.0}
                                                   };


        public laplace(ref Bitmap bmpIn)
        {
            Bitmap BmpOut = null;
            BitmapProcessing btprc = new BitmapProcessing();
            btprc.aplicaConvolutionFilter(filterLaplace, 5, 5, bmpIn, ref BmpOut);
            bmpIn = null;
            bmpIn = new Bitmap(BmpOut);
        }

    } // class laplace
    //*********************************************************************************************

    /// <summary>
    /// Cor padrao HSV. [h]:tonalidade [s]:saturação [v]: valor de Intensidade.
    /// [h]: um tipo de cor; 
    /// [s]: a saturação para um tipo de cor;
    /// [v]: a intensidade de brilho para o tipo de cor.
    /// O padrão HSV forma um cone, onde o circulo são a tonalidade (a variacao em graus),
    /// e a saturacao (a variacao em raio no circulo). A altura do cone seria o brilho, ou
    /// intensidade da cor. Na verdade, o que precisamos e' calcular a lateral do cone, para
    /// determinarmos a escala de luminosidade de uma mesma cor. a base seria a relacao entre
    /// [saturacao] e [tonalidade], que nos daria um lado do triangulo reto. a altura do trian-
    /// gulo seria a [intensidade:v]. Para calcular a luminosidade, calculamos a hipotenusa do
    /// triangulo formado por (esta) com (a [altura]) e (com lado da [saturacao] e [tonalidade]).
    /// Isto nos dá uma escala direta de intensidade de cor.
    /// </summary>
    public class ColorHSV
    {
        public float h { get; set; }
        public float s { get; set; }
        public float v { get; set; }
        public double angle { get; set; }


        /// <summary>
        /// construtor. Aceita os parametros normais do padrao HSV, e calcula o angulo
        /// da escala de tonalidade para a cor.
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="sat"></param>
        /// <param name="val"></param>
        public ColorHSV(float hue, float sat, float val)
        {
            this.h = hue;
            this.s = sat;
            this.v = val;


            // calcula o angulo associado a cor no padrao HSV.
            conversorRGBtoHSV cv = new conversorRGBtoHSV();
            Color corRGB = cv.UpdateRGB(this);
            this.angle = calcAngleHSV(this);
        } // ColorHSV()

        /// <summary>
        /// CALCULA O ANGULO DE UMA COR NO PADRAO HSV. SE DUAS CORES
        /// TIVERE ANGULOS MUITO PARECIDOS, A PROBABILIDADE DE SEREM
        /// DA MESMA COR, POREM DE LUMINOSIDADE DIFERENTE, E'  MUITO
        /// ALTA. USADA PARA RETIRAR OBJETOS DE CENAS DE IMAGEM.
        /// </summary>
        /// <param name="cor">cor no formato RGB.</param>
        /// <returns>retorna o angulo que representa a cor
        ///          no formato HSV </returns>
        private double calcAngleHSV(corHSV.ColorHSV cor)
        {
            ColorHSV corHSV = cor;

            double lado = Math.Sqrt(corHSV.h * corHSV.h + corHSV.s * corHSV.s);
            double angle = 0.0;

            if (Math.Abs(lado) < 0.001)
                angle = Math.PI / 2;
            else
                angle = Math.Atan(corHSV.v / lado);
            return (angle);

        } // calcAngleHSV()

        /// <summary>
        /// Se precisarmo saber com exatidao a luminosidade de uma cor, pelo
        /// padrao HSV e' possivel determina-la. Baseia-se no conceito do mo-
        /// delo HSV de cor. A luminosidade real é um valor na mesma reta in-
        /// clinada do feixe que parte da oridem do cone de cor HSV até     o
        /// circulo base. Nao e' o valor [v] do padrao HSV, pois este é a reta
        /// até o centro do circulo.
        /// </summary>
        /// <param name="cor">cor no formato padrao RGB</param>
        /// <returns>retorna a luminosidade da cor.</returns>
        private double calcLuminosidade(Color cor)
        {
            conversorRGBtoHSV cv = new conversorRGBtoHSV();
            ColorHSV corHSV = cv.UpdateHSV(cor);
            double lado = Math.Sqrt(corHSV.h * corHSV.h + corHSV.s * corHSV.s);
            double lum = Math.Sqrt(corHSV.v * corHSV.v + lado * lado);
            return (lum);
        } // calcLuminosidade().

    } // class ColoHSBV
    /// <summary>
    /// classe utilizada para comparar dois objetos corHSV.
    /// </summary>
    public class corHSVComparer : IComparer<ColorHSV>
    {
        int IComparer<ColorHSV>.Compare(ColorHSV ca1, ColorHSV ca2)
        {
            if (ca1.h > ca2.h)
                return (+1);
            if (ca1.h < ca2.h)
                return (-1);
            return (0);
        } // Compare()
    } // class corEAnguloComparer


    /// <summary>
    /// faz conversoes de padroes RGB para HSV e vice-versa.
    /// </summary>
    public class conversorRGBtoHSV
    {
        /// <summary>
        /// Updates the RGB color from the HSV
        /// </summary>
        public Color UpdateRGB(ColorHSV corHSV)
        {
            int conv;
            double hue, sat, val;
            int basex;
            hue = (float)corHSV.h / 100.0f;
            sat = (float)corHSV.s / 100.0f;
            val = (float)corHSV.v / 100.0f;
            int r = 0, g = 0, b = 0;
            if ((float)corHSV.s == 0) // Acromatic color (gray). Hue doesn't mind.
            {
                conv = (ushort)(255.0f * val);
                r = b = g = conv;
                return (Color.FromArgb(r, g, b));
            }

            basex = (ushort)(255.0f * (1.0 - sat) * val);

            switch ((ushort)((float)corHSV.h / 60.0f))
            {
                case 0:
                    r = (ushort)(255.0f * val);
                    g = (ushort)((255.0f * val - basex) * (corHSV.h / 60.0f) + basex);
                    b = basex;
                    break;

                case 1:
                    r = (ushort)((255.0f * val - basex) * (1.0f - ((corHSV.h % 60) / 60.0f)) + basex);
                    g = (ushort)(255.0f * val);
                    b = basex;
                    break;

                case 2:
                    r = basex;
                    g = (ushort)(255.0f * val);
                    b = (ushort)((255.0f * val - basex) * ((corHSV.h % 60) / 60.0f) + basex);
                    break;

                case 3:
                    r = basex;
                    g = (ushort)((255.0f * val - basex) * (1.0f - ((corHSV.h % 60) / 60.0f)) + basex);
                    b = (ushort)(255.0f * val);
                    break;

                case 4:
                    r = (ushort)((255.0f * val - basex) * ((corHSV.h % 60) / 60.0f) + basex);
                    g = basex;
                    b = (ushort)(255.0f * val);
                    break;

                case 5:
                    r = (ushort)(255.0f * val);
                    g = basex;
                    b = (ushort)((255.0f * val - basex) * (1.0f - ((corHSV.h % 60) / 60.0f)) + basex);
                    break;
            }
            Color corRetorno = Color.FromArgb(r, g, b);
            return (corRetorno);
        } // void UpdateRGB()



        /// <summary>
        /// Updates the HSV color from the RGB
        /// </summary>
        public ColorHSV UpdateHSV(Color cor)
        {
            int max, min, delta;
            int temp;

            int r = cor.R;
            int g = cor.G;
            int b = cor.B;

            float h;
            float s;
            float v;

            max = MAX(r, g, b);
            min = MIN(r, g, b);
            delta = max - min;

            if (max == 0)
            {
                s = h = v = 0;
                return (new ColorHSV(h, s, v));
            }


            v = (int)((double)max / 255.0 * 100.0);
            s = (int)(((double)delta / max) * 100.0);

            if (r == max)
                temp = (int)(60 * ((g - b) / (double)delta));
            else if (g == max)
                temp = (int)(60 * (2.0 + (b - r) / (double)delta));
            else
                temp = (int)(60 * (4.0 + (r - g) / (double)delta));

            if (temp < 0)
                h = temp + 360;
            else
                h = temp;
            return (new ColorHSV(h, s, v));
        } // void UpdateHSV()

        /// <summary>
        /// calcula qual dos parametros e' o maximo.
        /// </summary>
        /// <param name="a">parametro 1.</param>
        /// <param name="b">parametro 2.</param>
        /// <param name="c">parametro 3.</param>
        /// <returns>retorna o maximo dos tres parametros.</returns>
        private int MAX(int a, int b, int c)
        {
            if ((a > b) && (a > c))
                return (a);
            else
                if (c > b)
                    return (c);
                else
                    return (b);

        } // MAX()
        /// <summary>
        /// calcula qual dos parametros e' o minimo
        /// </summary>
        /// <param name="a">parametro 1.</param>
        /// <param name="b">parametro 2.</param>
        /// <param name="c">parametro 3.</param>
        /// <returns>retorna o minimo dos tres parametros.</returns>
        private int MIN(int a, int b, int c)
        {
            if ((a < b) && (a < c))
                return (a);
            else
                if (c < b)
                    return (c);
                else
                    return (b);
        }

    } // class CONVERSOR_RGB_HSV

     


    public class disivisaoDeClustersPorMetodosEstatisticos
    {
        public static int contadorParaMinimoDeCoresHSVparaUmNovoCluster = 15;
  
        class indicesIniciaisEFinais
        {
            public int ini { get; set; }
            public int fini { get; set; }
            /// <summary>
            /// construtor de 0 argumentos. inicializa os 
            /// dados do novo objeto com valores default 
            /// igual a 0.
            /// </summary>
            public indicesIniciaisEFinais()
            {
                this.ini = 0;
                this.fini = 0;
            } // construtor com 0 argumentos.
            
            /// <summary>
            /// construtor com 2 argumentos, preenchendo 
            /// os valores das propriedades do objeto.
            /// </summary>
            /// <param name="indiceInicial">índice dito inicial.</param>
            /// <param name="indiceFinal">índice dito final.</param>
            public indicesIniciaisEFinais(int indiceInicial, int indiceFinal)
            {
                this.ini = indiceInicial;
                this.fini = indiceFinal;
            } // construtor de 2 argumentos.
        } // indicesIniciaisEFinais

        /// <summary>
        /// retorna uma lista de lista de cores hsv. Cada lista de cores hsv
        /// tem suas componentes [h]  hue um valor  de diferença entre si, e
        /// consecutivas, menor  que  o  valor  [incAngulo],  calculado pela
        /// propriedade raio.
        /// </summary>
        /// <param name="lstcoreshsv">lista de cores hsv original.</param>
        /// <param name="raio">utlizado no calculo da diferença entre angulos.</param>
        /// <returns></returns>
        public List<List<ColorHSV>> 
            pre_calculoDivisaoDeClusterPorMetodosEstatisticos(
                                                              List<ColorHSV> lstcoreshsv,
                                                              double raio)
        {
            corHSVComparer cmpH = new corHSVComparer();
            lstcoreshsv.Sort(cmpH);

            List<List<ColorHSV>> lstOfLstCores= new List<List<ColorHSV>>();
            // incremento de angulo. cores hsv consecutivas que possuem diferença entre sua componente
            // hue [h] menor que este valor, serão agrupados em uma só lista.
            double incAngle = 360.0 / (2.0 * Math.PI * raio);
            int ini = 0;
            int fini = 0;
            int x=0;
            List<ColorHSV> lstCurrente = new List<ColorHSV>();
            // percorre a lista, agrupando em listas cores hsv que possuem
            // a componente [h] hue uma diferença menor que [incAngle].
            // [incAgle] foi estudado em projeto anterior: visualizadorDeCoresHSV,
            // disposto no REPOSITORIO de Projetos Terminados.
            while (ini < lstcoreshsv.Count)
            {
                fini=ini;
                // faz a malha while correr até que a diferença entre duas cores hsv consecutivas
                // seje maior que o valor [incAngle].
                while ((Math.Abs(lstcoreshsv[fini].h - lstcoreshsv[fini + 1].h) < incAngle) &&
                      (fini < lstcoreshsv.Count)) 
                {
                    fini++;
                } // while
               
                // agrupa as cores de diferença menor, em lista de cores hsv currente.
                for (x = ini; x < fini; x++)
                {
                    lstCurrente.Add(lstcoreshsv[x]);
                } // for x
                // junta à lista das listas, a lista currente de cores de diferença menor
                if (lstcoreshsv.Count > 0)
                {
                    lstOfLstCores.Add(lstCurrente.ToList<ColorHSV>());
                    lstCurrente.Clear();
                } //if
                ini = fini;
            } // for ini
            // retorna a lista de lista de cores hsv calculada.
            return (lstOfLstCores);

        } // retornaResumoCoresHSV()

        /// <summary>
        /// Faz uma aproximação de cores HSV, agrupadas pelos seus componentes [h] hue,
        /// calculando novas cores,uma para cada grupo, como sendo a média das compen-
        /// tes do grupo de cores.
        /// </summary>
        /// <param name="lstOfLstCores"></param>
        private void calculaListaResumida(List<List<ColorHSV>> lstOfLstCores)
        {
            List<float> mediaComponenteH = new List<float>();
            List<float> mediaComponenteS = new List<float>();
            List<float> mediaComponenteV = new List<float>();
            float mediaH = 0.0F;
            float mediaS = 0.0F;
            float mediaV = 0.0F;
            List<ColorHSV> coresResumidas = new List<ColorHSV>();
            int indiceLista = 0;
            int indiceCores = 0;
            for (indiceLista = 0; indiceLista < lstOfLstCores.Count; indiceLista++)
            {
                mediaComponenteH.Clear();
                mediaComponenteS.Clear();
                mediaComponenteV.Clear();
                // acrescenta à lista de cada componente, os componentes das cores HSV vindas de [lstOfLstCores].
                for (indiceCores = 0; indiceCores < lstOfLstCores[indiceLista].Count; indiceCores++)
                {
                    mediaComponenteH.Add(lstOfLstCores[indiceLista][indiceCores].h);
                    mediaComponenteS.Add(lstOfLstCores[indiceLista][indiceCores].s);
                    mediaComponenteV.Add(lstOfLstCores[indiceLista][indiceCores].v);
                } // for indiceCores
                mediaH = media(0, lstOfLstCores[indiceLista].Count, mediaComponenteH);
                mediaS = media(0, lstOfLstCores[indiceLista].Count, mediaComponenteS);
                mediaV = media(0, lstOfLstCores[indiceLista].Count, mediaComponenteV);

                coresResumidas.Add(new ColorHSV(mediaH, mediaS, mediaV));
            } // for indiceLista

        } // calculaListaResumida()

        // este algoritmo é mais aperfeiçoado que o anterior, faz uma
        // varredura buscando a media e desvio padrao dos pontos.
        // Em seguida, retira os pontos que estejam fora da faixa  da
        // média+-desvioPadrao. Depois, agrega os pontos em  clusters.
        // A grandeza utilizada é a intensidade hue do padrão HSB.
        // Este algoritmo foi extraído da observação do gráfico de
        // intensidades hue, em C#.NET
        // nota informal: era intensão original deste algoritmo depender
        // do usuário informar o número de clusters como entrada,    mas
        // a linha de raciocínio foi se desenvolvendo e chegou-se a  uma
        // conclusão que não precisava de tal entrada! Seria automatiza-
        // do. O fator de agregação pela media e desvio padrão a  partir
        // de uma grandeza de mensuração é uma técnica útil e proveitosa.
        public static List<List<ColorHSV>>
            tecnicaDeDivisaoDeClustersPorDesvioPadrao(Bitmap cena)
        {
            // extrai cores no formato RGB a partir de uma imagem [cena]
            // inicializa o extrator de cores.
            extratorDeCoresRGBdeUmaImagem extratCores= new extratorDeCoresRGBdeUmaImagem();
            // retira a paleta de cores com o extrator
            List<Color> lstCoresRGB = extratCores.ExtraiCores(cena);

            // cria um objeto conversor de cores RGV e HSV.
            conversorRGBtoHSV conversor= new conversorRGBtoHSV();
            // cria uma lista de cores hsv.
            List<ColorHSV> cores = new List<ColorHSV>();
            // cria a lista de cores HSV a partir da conversão da lista de cores HSV (para RGB).
            for (int corrgb = 0; corrgb < lstCoresRGB.Count; corrgb++)
                cores.Add(conversor.UpdateHSV(lstCoresRGB[corrgb]));

            // ordena a lista de cores hsv de acordo com a quantidade dada pela componente hue [h]
            // das cores HSV.
            // cria o comparador entre duas cores hsv consecutivas.
            corHSV.corHSVComparer cmpHSV = new corHSVComparer();
            // faz a ordenação.
            cores.Sort(cmpHSV);
         
            ColorHSV corCurrente = new ColorHSV(0.0F, 0.0F, 0.0F);
            ColorHSV corAnterior = cores[0];
            int contadorIndiceCores = 0;
            int i;
            int ini = 0;
            int fini = ini + 1;
            List<float> listapadrao = new List<float>();
            for (i = ini; 0 < cores.Count; i++)
                listapadrao.Add(cores[i].h);

            List<indicesIniciaisEFinais> lstListaDeIndicesDeClustersDeCores =
                new List<indicesIniciaisEFinais>();
            while (contadorIndiceCores < cores.Count)
            {
                //*********************************************************************
                // coleta dados estatísticos para posterior retirada de
                // pontos de clusters "falso-alarme".
                // os cálculos são feito na lista com cores indo
                // do inicio+1 do cluster anterior até quando  a
                // diferença ultrapassar os 95% dos dados dentro
                // do novo cluster.

                double med = media(ini, fini, listapadrao);
                double dsp = desvioPadrao(ini, fini, listapadrao);
                //**********************************************************************
                // seta a nova cor currente, utilizando [contadorIndiceCores] como índice
                // de posição.
                corCurrente = cores[contadorIndiceCores];

                // se os dados da cor currente ultrapassar 95% das amostras 
                // até então calculadas, cria um novo indice, mostrando que
                // é caso de um novo cluster
                if (((corCurrente.h - corAnterior.h) > (med - 2.0F * dsp)) &&
                    ((corCurrente.h - corAnterior.h) < (med + 2.0F * dsp)))
                {
                    if (fini > cores.Count)
                    {
                        // adiciona à lista de índices o inicio e o fim possíveis de serem alcançados.
                        lstListaDeIndicesDeClustersDeCores.Add(new indicesIniciaisEFinais(ini, cores.Count - 1));
                        break;
                    } // if fini

                    // adiciona um novo cluster delimitado por [ini],[fini]
                    // adicona à lista de índice os índices do cluster que acabou de ser delimitado.
                    lstListaDeIndicesDeClustersDeCores.Add(new indicesIniciaisEFinais(ini, fini));
                    // iguala os indices [ini] e [fini], para comportar um novo cluster.
                    fini = ini;
                } // if
                else
                    // este caso é quando a cor currente é agregada ao 
                    // cluster currente, modificando seu indice  final
                    fini++;
                contadorIndiceCores++;
                // seta a cor anterior como sendo a corCurrente
                corAnterior = corCurrente;
            } // while()


            //******************************************************************************************************
            // retira sequencia de pontos
            // de [contadorParaMinimoDeCoresHSVparaUmNovoCluster]
            // ou menos em sequencia, que seriam contadas como um novo cluster,
            // mas que representam cores isoladas, uma anomalia constatada   a-
            // través de experiências anteriores.
            List<indicesIniciaisEFinais> lstFinalIndices= new List<indicesIniciaisEFinais>();
            i = 0;
            while (i < lstListaDeIndicesDeClustersDeCores.Count)
            {
                // se a diferença do numero de matizes for maior que
                // [contadorParaMinimoDeCoresHSVparaUmNovoCluster]
                // cria um novo objeto de indices iniciais e finais,
                //  indicando o inicio 
                // de um novo cluster.
                if ((lstListaDeIndicesDeClustersDeCores[i].fini - 
                     lstListaDeIndicesDeClustersDeCores[i].ini) >
                     contadorParaMinimoDeCoresHSVparaUmNovoCluster)
                {
                    lstFinalIndices.Add(new indicesIniciaisEFinais(lstListaDeIndicesDeClustersDeCores[i].ini,
                                                                   lstListaDeIndicesDeClustersDeCores[i].fini));
                } // if

            } // while i
            //************************************************************************************************
            // forma os clusters, tendo como limites os
            // pontos que estejam muito acima da media +- desvioPadrao,
            // e clusteres que tem mais cores que o   minimo  estabele-
            // cido em [contadorParaMinimoDeCoresHSVparaUmNovoCluster].
            int index = 1; int ci = 0; int cf = 0; int indexCluster = 0;
            List<List<ColorHSV>> listaClusters= new List<List<ColorHSV>>();
            foreach (indicesIniciaisEFinais indices in lstFinalIndices)
            {
                ci = indices.ini;
                cf = indices.fini;
            
                // cria uma nova lista de cores HSV, dentro da lista de listas.
                listaClusters.Add(new List<ColorHSV>());
                // índice que aponta para a última lista de cores HSV,
                // que acabou de ser criada.
                indexCluster=listaClusters.Count-1;

                // cria os clusteres a partir dos indices de limite calculados anteriormente.
                for (i = ci;i <= cf; i++)
                    listaClusters[indexCluster].Add(cores[i]);
                index++;
            } // while

            return (listaClusters);
        } // tecnicaDeDivisaoDeClusters2()

        /// <summary>
        /// calcula o desvio padrão de uma lista de floats.
        /// </summary>
        /// <param name="ini">índice inicial para o processamento da lista.</param>
        /// <param name="fini">índice fina para o processamento da lista.</param>
        /// <param name="v">lista de números, da qual será estraída o desvio padrão.</param>
        /// <returns>retorna o desvio padrão da lista de entrada, dentro dos indices [ini] e [fini].</returns>
        private static float desvioPadrao(int ini, int fini, List<float> v)
        {
            float soma = 0.0F;
            float media = 0.0F;
            int i;
            for (i = ini; i < fini; i++)
            {
                soma += v[i];
            } // for i
            if (fini == ini)
                media = 0;
            else
                media = soma / (float)(fini - ini);

            List<float> y = new List<float>();
            for (i = ini; i < fini; i++)
            {
                y.Add((v[i] - media) * (v[i] - media));
            } // for i
            float s = 0.0F;
            for (i = 0; i < y.Count; i++)
            {
                s += y[i];
            } // for i
            if (y.Count == 0) return (-1.0F);
            float desviopadrao = (float)Math.Sqrt(s / ((float)(y.Count - 1)));

            return desviopadrao;

        } // desvioPadrao()
        /// <summary>
        /// calcula a média de uma lista de valores numéricos, dentro de uma faixa de índices.
        /// </summary>
        /// <param name="ini">índice inicial para processamento da lista.</param>
        /// <param name="fini">índice final para processamento da lista.</param>
        /// <param name="v">lista de valores a serem processados para formar a média.</param>
        /// <returns>retorna o valor mediano da lista de valores, limitada pelos indices [ini] e [fini].</returns>
        private static float media(int ini, int fini, List<float> v)
        {
            float s = 0.0F;
            for (int i = 0; i < v.Count; i++)
                s += v[i];
            s /= ((float)v.Count);
            return s;
        }


    } // class disivisaoDeClustersPorMetodosEstatisticos

    /// <summary>
    /// classe para EXTRAIR cores de uma imgagem, formando uma palete retornada na
    /// forma de lista.
    /// </summary>
    public class extratorDeCoresRGBdeUmaImagem
        {
            /// <summary>
            /// matriz para registro de componentes de cores [R,G,B] repetidos.
            /// </summary>
            public Dictionary<Color, int> dctnry = new Dictionary<Color, int>();

            /// <summary>
            /// extrai as cores RGB de uma Imagem, na quantidade 1 de cada cor,
            /// ou seja, o algoritmo extrai os exemplares das cores da imagem,
            /// um para cada cor presente.
            /// </summary>
            /// <param name="cena"></param>
            /// <returns></returns>
            public List<Color> ExtraiCores(Bitmap cena)
            {

                for (int y = 0; y < cena.Height; y++)
                {
                    for (int x = 0; x < cena.Width; x++)
                    {
                        this.registraCor(cena.GetPixel(x, y));
                    } // for x
                } // for y
                return (this.extraiCoresRGB());
            } //  extratorDeCoresRGBdeUmaImagem()


            /// <summary>
            /// se a cor estiver presente na lista de cores, retorna [true] 
            /// (e' repetido), se nao for repetida, retorna  [false].
            /// </summary>
            /// <param name="cor">cor a ser testada se e' repetida.</param>
            /// <returns>[true] se a cor for repetida, [false] se nao estiver na matriz.</returns>
            public bool repeticaoCor(Color cor)
            {
                int n = -1;
                bool s = dctnry.TryGetValue(cor, out n);
                if (n == (-1))
                {
                    return true;
                } // if n==1
                return false;
            } // bool repeticaoCor()
            /// <summary>
            /// registra uma cor, se ela já não estiver dentro da lista de cores.
            /// </summary>
            /// <param name="cor">cor a ser registrada.</param>
            public void registraCor(Color cor)
            {
                int n = -1;
                this.dctnry.TryGetValue(cor, out n);
                if (n == 0)
                {
                    this.dctnry.Add(cor, 1);
                } // for n
            } // void registraCor()

            /// <summary>
            /// retorna uma lista de cores RGB das cores registradas
            /// até o currente momento.
            /// </summary>
            /// <returns></returns>
            public List<Color> extraiCoresRGB()
            {
                List<Color> lstcoresRGB = new List<Color>();
                for (int index = 0; index < this.dctnry.Values.Count; index++)
                {
                    lstcoresRGB.Add(this.dctnry.Keys.ElementAt<Color>(index));
                } // for index
                return lstcoresRGB;
            } // extraiCoresRGB()

        } // processamentoImagensHSV
} // namespace