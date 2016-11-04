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

/// NAMESPACE DE PROCESSAMENTO DE IMAGENS, PREPARATORIO PARA O MAPEAMENTO 3D.
namespace processamento
{
    /// <summary>
    /// VETOR DE CORDENADAS ESPACIAIS DE NUMEROS INTEIROS, COM COMPONENTE COR.
    /// </summary>
    public class vetor2Dint
    {
        public int x { get; set; }
        public int y { get; set; }
        public Color cor { get; set; }
        public vetor2Dint(int xx, int yy, Color ccor)
        {
            this.x = xx;
            this.y = yy;
            this.cor = ccor;
        }
        public vetor2Dint(vetor2Dint v)
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
        public List<vetor2Dint> cordenadasObjetoCluster = null;
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
            public List<vetor2Dint> cord2D = new List<vetor2Dint>();
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
            this.cordenadasObjetoCluster = new List<vetor2Dint>();
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
            this.cordenadasObjetoCluster = new List<vetor2Dint>();

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
                            this.cordenadasObjetoCluster.Add(new vetor2Dint(clusters[cluster].cord2D[x]));
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
            /// SE-RE OS DADOS DE CADA OBJETO CLUSTER EM UMA IMAGEM.


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
                    clusters[keyX * 256 + keyY].cord2D.Add(new vetor2Dint(x, y, bmpIn.GetPixel(x, y)));
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
        private void detectaFronteiras(List<vetor2Dint> cords, ref int x1,
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
            CONVERSOR_RGB_HSV cv = new CONVERSOR_RGB_HSV();
            Color corRGB = cv.UpdateRGB(this);
            this.angle = calcAngleHSV(corRGB);
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
        public double calcAngleHSV(Color cor)
        {
            CONVERSOR_RGB_HSV cv = new CONVERSOR_RGB_HSV();
            ColorHSV corHSV = cv.UpdateHSV(cor);

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
        public double calcLuminosidade(Color cor)
        {
            CONVERSOR_RGB_HSV cv = new CONVERSOR_RGB_HSV();
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
            if (ca1.angle > ca2.angle)
                return (+1);
            if (ca1.angle < ca2.angle)
                return (-1);
            return (0);
        } // Compare()
    } // class corEAnguloComparer

        
    /// <summary>
    /// faz conversoes de padroes RGB para HSV e vice-versa.
    /// </summary>
    public class CONVERSOR_RGB_HSV
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
            int r=0, g=0, b=0;
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
                return (new ColorHSV(h,s,v));
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
    

    /// <summary>
    /// faz o processamento de imagens segundo cores no padrao HSV, que tem 
    /// a grande vantagem de saber a luminosidade em seus calculos, pois es-
    /// te tem relacao direta com os componentes da cor neste padrao.
    /// </summary>
    public class processamentoImagensHSV
    {
        /// <summary>
        /// processa as cores de uma cena, reunindo-os em clusters de objetos. o parametro
        /// [DiffAngles] e' o que diferencia uma cor de outra.
        /// </summary>
        /// <param name="ena">cena a ser processada.</param>
        /// <param name="lstscoresObjetos">lista de objetos, cada um contendo uma lista
        ///                                de cores. Cada objeto tem uma unica cor.
        ///                                </param>
        public void processamentoImagem(Bitmap Cena, out List<Color[]> lstscoresObjetos,
            double DiffAngles)
        {
            int cor;

            List<Color> CoresCena= new List<Color>();
            CoresCena = extrai_CoresBitmap(Cena);
            
            // esta sera a lista com as cores de cada objeto.
            lstscoresObjetos = new List<Color[]>();
            
            
            // gera a lista de cores no padrao HSV.
            List<ColorHSV> coresHSV = new List<ColorHSV>();
            CONVERSOR_RGB_HSV cv_RGB_HSV = new CONVERSOR_RGB_HSV();
            for (cor = 0; cor < CoresCena.Count; cor++)
                coresHSV.Add(cv_RGB_HSV.UpdateHSV(CoresCena[cor]));

            // ordena a lista de cores, de acordo com os angulos (vide a implementacao da
            // interface IComparer na classe ColorHSV).
            coresHSV.Sort(new corHSVComparer());

            CONVERSOR_RGB_HSV cv = new CONVERSOR_RGB_HSV();
            List<Color> lstObjetoAtual = new List<Color>();
            lstObjetoAtual.Add(cv.UpdateRGB(coresHSV[0]));
            for (cor = 1; cor < coresHSV.Count; cor++)
            {
                // se os angulos estiverem muito proximos, e' que provavelmente sao da mesma
                // cor, entao soma para uma mesma lista de cores de um mesmo objeto.
                if ((coresHSV[cor].angle - coresHSV[cor - 1].angle) < DiffAngles)
                    lstObjetoAtual.Add(cv.UpdateRGB(coresHSV[cor]));
                else
                {
                    // se ultrapassar o angulo, guarda o objeto lista corrente de cores
                    // para a lista de objetos e
                    // limpa a lista de cores corrente. Em seguida, acrescenta a cor cor-
                   // rente para a lista de cores.
                    lstscoresObjetos.Add((Color[])lstObjetoAtual.ToArray().Clone());
                    lstObjetoAtual.Clear();
                    lstObjetoAtual.Add(cv.UpdateRGB(coresHSV[cor]));
                } // else

            } // for cor
          } // processamentoImagem.


        /// <summary>
        /// extrai as cores do Bitmap, utilizando metodos rapidos, a custo de memoria.
        /// </summary>
        /// <param name="Cena">Bitmap a ter as cores retirada.</param>
        /// <returns>retorna a lista de cores do Bitmap, sem repeticoes.</returns>
        public List<Color> extrai_CoresBitmap(Bitmap Cena)
        {
            // utiliza um objeto BitmapData para retirar os dados do Bitmap (como um buffer).
            BitmapData bmpdata = new BitmapData();
            // passa os dados do Bitmap para o BitmapData
            bmpdata = Cena.LockBits(new Rectangle(0, 0, Cena.Width, Cena.Height), ImageLockMode.ReadOnly,
                Cena.PixelFormat);

            // dados para copia de cores: ponteiro para o inicio do buffer, tamanho do buffer,
            // e definicao da matriz que contera as cores.
            IntPtr CenaPtr = bmpdata.Scan0;
            // stride: (width+complento ate' completar 8 bits [(stride%8)==0])
            int sizeBufBitmap = bmpdata.Height * bmpdata.Stride;
            byte[] Cores = new byte[sizeBufBitmap];

            // copia as cores do bitmap (contido no BitmapData [bmpdata]) para a matriz [Cores].
            System.Runtime.InteropServices.Marshal.Copy(CenaPtr, Cores, 0, sizeBufBitmap);

            // realiza a operacao de filtragem de cores repetidas: somente uma copia de cada
            // cor sera' colocada na lista a ser retornada.
            int x;
            // lista a ser retornada
            List<Color> lstCores = new List<Color>();
            // matriz de repeticoes: assegura que somente uma copia de cada cor sera
            // adicionada a lista
            matrizREPETICAOcores mtrRept = new matrizREPETICAOcores();
            for (x = 0; x < sizeBufBitmap; x += 3)
            {
                // compoe a cor a partir do buffer de cores.
                Color cor = Color.FromArgb(Cores[x], Cores[x + 1], Cores[x + 2]);

                // se a cor nao for repetida (nao estiver registrada na matriz de repeticoes),
                // adiciona a cor na lista de cores a ser retornada.
                if (mtrRept.repeticaoCor(cor) == false)
                {
                    lstCores.Add(cor);
                    // em seguida, registra a cor como repetida na matriz de repeticoes.
                    mtrRept.registraCor(cor);
                } // if mtrRept.
            } // for x
            // retorna a Lista de cores, sem repeticoes..
            return (lstCores);
        } // extrai_CoresBitmap()

      
        /// <summary>
        /// classe interna para registrar repeticoes de cores. utiliza muita memoria..
        /// </summary>
        public class matrizREPETICAOcores
        {
            /// <summary>
            /// matriz para registro de componentes de cores [R,G,B] repetidos.
            /// </summary>
            public static byte[, ,] matrizRepeticao = new byte[256, 256, 256];

            /// <summary>
            /// se a cor estiver na matriz entao [matrizRepeticao[R,G,B]=1],neste
            /// caso retorna [true] (e' repetido), se nao for repetida,   retorna 
            /// [false].
            /// </summary>
            /// <param name="cor">cor a ser testada se e' repetida.</param>
            /// <returns>[true] se a cor for repetida, [false] se nao estiver na matriz.</returns>
            public bool repeticaoCor(Color cor)
            {
                if (matrizRepeticao[cor.R, cor.G, cor.B] == 1)
                    return (true);
                else
                    return (false);
            } // bool repeticaoCor()
            /// <summary>
            /// registra a cor na matriz como repetida, ou seja [matrizRepeticao[R,G,B]=1].
            /// </summary>
            /// <param name="cor">cor a ser registrada.</param>
            public void registraCor(Color cor)
            {
                matrizREPETICAOcores.matrizRepeticao[cor.R, cor.G, cor.B] = 1;
            } // void registraCor()

            
        } // class matrizREPETICAOcores


        // este algoritmo é mais aperfeiçoado que o anterior, faz uma
        // varredura buscando a media e desvio padrao dos pontos.
        // Em seguida, retira os pontos que estejam fora da faixa  da
        // média~desvioPadrao. Depois, agrega os pontos em  clusters.
        // A grandeza utilizada é a intensidade hue do padrão HSB.
        // Este algoritmo foi extraído da observação do gráfico de
        // intensidades hue, em C#.NET
        // nota informal: era intensão original deste algoritmo depender
        // do usuário informar o número de clusters como entrada,    mas
        // a linha de raciocínio foi se desenvolvendo e chegou-se a  uma
        // conclusão que não precisava de tal entrada! Seria automatiza-
        // do. O fator de agregação pela media e desvio padrão a  partir
        // de uma grandeza de mensuração é uma técnica útil e proveitosa.
        public List<List<ColorHSV>> tecnicaDeDivisaoDeClusters2(ColorHSV[] cores)
        {
            // cria uma coleção ordenada para guardar valores
            // [diferença cores hue]-->[cor hsb].
            Dictionary<float, int> hshtbl = new Dictionary<float, int>();
            int c;
            hshtbl.Add(new Integer(0),0);
            for (c = 1; c < cores.length; c++)
                hshtbl.put(new Float(cores[c].h - cores[c - 1].h), c);
            //*********************************************************************
            // coleta dados estatísticos para posterior retirada de
            // pontos de clusters "falso-alarme".
            List<ColorHSV> listapadrao = new List<ColorHSV>();
            foreach (ColorHSV cor in hshtbl.Values)
                listapadrao.Add(cores[cor]);
            
            double med = this.mediaAritmetica(listapadrao);
            double dsp = this.desvioPadrao(listapadrao);

            //**********************************************************************
            // retira pontos que estejam muito acima da media~desvioPadrao.
            List<ColorHSV> listaCores = new List<ColorHSV>();
            ColorHSV corpop = new ColorHSV(0.0F, 0.0F, 0.0F);
            ColorHSV corcurrente = listapadrao[0];
            int contadorIndiceCores = 1;
            while (contadorIndiceCores<listapadrao.Count)
            {
                corpop = listapadrao[contadorIndiceCores];
                if (((corpop.h - corcurrente.h) > (med - 2.0F * dsp)) &&
                   ((corpop.h - corcurrente.h) < (med + 2.0F * dsp)))
                    listaCores.add(contadorIndiceCores);
                contadorIndiceCores++;
                corcurrente = new corHSB(corpop);
            } // while()

            //******************************************************************************************************
            // retira sequencia de pontos de 2 ou mais em sequencia, que estejam muito acima da media+-desvioPadrao.
            int i = 1;
            List<int> listaFinal= new List<int>();
            listaFinal.Add(0);
            while (i < listapadrao.Count)
            {
                if ((listaCores[i] - listaCores[i - 1]) > 2)
                    listaFinal.Add(i);
            } // while i
            //************************************************************************************************
            // forma os clusters, tendo como limites os pontos que estejam muito acima da media+-desvioPadrao.
            int index = 1; int ci = 0; int cf = 0; int indexCluster = 0;
            List<List<ColorHSV>> listaClusters= new List<List<ColorHSV>>();
            while (index < listaCores.Count)
            {
                ci = listaCores[index - 1];
                cf = listaCores[index];
            
                listaClusters.Add(new List<ColorHSV>());
                indexCluster=listaClusters.Count-1;
                for (int c = ci; c <= cf; c++)
                    listaClusters[indexCluster].Add(listapadrao[c]);
                index++;
            } // while

            return (listaClusters);
        } // tecnicaDeDivisaoDeClusters2()

    } // processamentoImagem


} // namespace processamento
