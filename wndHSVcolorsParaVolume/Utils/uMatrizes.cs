using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Utils
{
    // **************************************************************************
    //                              CLASSE MATRIZ
    // **************************************************************************                             
    // classe que encapsula uma matriz bidimensional- base
    // para calculos. contem um aleatorizador para
    // preencimento randomico de elementos.
    // Metodos publicos:
    // ->Matriz(lin,col) : construtor;
    // ->Matriz(lin,col,params double[] elementos): construtor com vetor de elementos iniciais.
    // ->Matriz.MatrizInversa(A): calcula a Matriz inversa da Matriz A.
    // ->Identidade(): calcula a Matriz identidade da matriz que chamou o método.
    // ->operadores +,-,/,*: operadores para operações matriciais.
    // ->ToString(): coloca no console os elementos da Matriz
    //   que chamou o metodo.
    // ->preencheMatriz(intervalo): prenche uma Matriz com 
    //   valores aleatorios variando de [0..intervalo*qtLin].
    // ->Sort(): ordena uma matriz se esta tiver lin=1 ou col=1.
    // ->Clone(): devolve uma copia da matriz que chamou o metodo;
    // ->Determinante(Matriz M): encontra o determinante da matriz M.

    /// <summary>
    /// classe que encapsula o conceito matematico de matriz.
    /// </summary>
    public class Matriz
    {
        private double[,] matriz;
        public int qtLin;
        public int qtCol;
        private static Random aleatorizador = new Random(seedRandom);
        public static int seedRandom = 10000;

        public void setElemento(int lin, int col, double el)
        {
            this.matriz[lin, col] = el;

        } // void setElemento()
        public static Matriz Identidade(int dim)
        {
            Matriz I = new Matriz(dim, dim);
            for (int l = 0; l < dim; l++)
                for (int c = 0; c < dim; c++)
                {
                    if (l == c)

                        I.matriz[l, c] = 1.0;
                    else
                        I.matriz[l, c] = 0.0;
                } // for int c
            return (I);
        } //Identidade()

        public double getElemento(int lin, int col)
        {
            return (this.matriz[lin, col]);
        } // double getElemento()

        public Matriz(int lin, int col, params double[] elementos)
        {
            this.qtLin = lin;
            this.qtCol = col;
            this.matriz = new double[this.qtLin, this.qtCol];
            if (elementos.Length == (this.qtLin * this.qtCol))
            {
                int contadorElementos = 0;
                try
                {
                    for (int l = 0; l < this.qtLin; l++)
                        for (int c = 0; c < this.qtCol; c++)
                            this.matriz[l, c] = elementos[contadorElementos++];
                }
                catch { throw new Exception("Falta ou exesso de parametros no construtor. "); }
            } // if
        } // Matriz()


        public Matriz()
        {
            this.matriz = new double[1, 1];
            this.qtCol = 1;
            this.qtLin = 1;
        } // Matriz()

        public Matriz(int lin, int col)
        {
            this.qtLin = lin;
            this.qtCol = col;
            this.matriz = new double[this.qtLin,this.qtCol];

        } // Matriz()

        /// construtor basico para classes herdeiras.
        /// aceita o numero de linhas e colunas da matriz.
        /// <param name="lin">numero de linhas da matriz.</param>
        /// <param name="col">numero de colunas da matriz.</param>
        public void initMatriz(int lin, int col)
        {
            this.qtLin = lin;
            this.qtCol = col;
            this.matriz = new double[this.qtLin, this.qtCol];
        } // Matriz()

        /// metodo controle para construtores e clone. Copia para
        /// a matriz que chamou o metodo a matriz parametro.
        /// <param name="M">matriz parametro a ser copiada.</param>
        public void initMatriz(Matriz M)
        {
            this.qtLin = M.qtLin;
            this.qtCol = M.qtCol;
            this.matriz = new double[this.qtLin, this.qtCol];
            int lin, col;
            for (lin = 0; lin < this.qtLin; lin++)
                for (col = 0; col < this.qtCol; col++)
                    this.matriz[lin, col] = M.matriz[lin, col];

        } // Matriz()
        /// retorna uma copia da matriz que chamou o metodo.
        public Matriz Clone()
        {
            Matriz N = new Matriz();
            N.initMatriz(this);
            return (N);
        } // Clone()

        /// multiplica duas matrizes, e retorna como produto uma matriz.
        /// <param name="A">matriz operando 1.</param>
        /// <param name="B">matriz operando 2.</param>
        public static Matriz MulMatriz(Matriz A, Matriz B)
        {
            Matriz C = new Matriz(A.qtLin, B.qtCol);
            int lin, col, k;
            for (lin = 0; lin < C.qtLin; lin++)
            {
                for (col = 0; col < C.qtCol; col++)
                {
                    C.matriz[lin, col] = 0.0;
                    for (k = 0; k < A.qtCol; k++)
                        C.matriz[lin, col] += A.matriz[lin, k] * B.matriz[k, col];
                } // for col

            } // for lin

            return (C);
        } // MulMatriz()


        /// <summary>
        /// soma duas matrizes. Operador de adição.
        /// </summary>
        /// <param name="A">Matriz Parametro.</param>
        /// <param name="B">Matriz Parametro.</param>
        /// <returns>retorna uma Matriz soma das matrizes [A] e [B].</returns>
        public static Matriz operator +(Matriz A, Matriz B)
        {
            try
            {
                Matriz C = new Matriz(A.qtLin, A.qtCol);
                for (int lin = 0; lin < A.qtLin; lin++)
                    for (int col = 0; col < A.qtCol; col++)
                    {
                        C.matriz[lin, col] = A.matriz[lin, col] + B.matriz[lin, col];
                    } // for int col
                return (C);
            } // try
            catch
            {
                return (null);
            } // catch

        } // Matriz operator +()

        /// <summary>
        /// subtrai duas matrizes. Operador de subtração.
        /// </summary>
        /// <param name="A">Matriz Parametro.</param>
        /// <param name="B">Matriz Parametro.</param>
        /// <returns>retorna uma Matriz diferença entre as matrizes [A] e [B].</returns>
        public static Matriz operator -(Matriz A, Matriz B)
        {
            try
            {
                Matriz C = new Matriz(A.qtLin, A.qtCol);
                for (int lin = 0; lin < A.qtLin; lin++)
                    for (int col = 0; col < A.qtCol; col++)
                    {
                        C.matriz[lin, col] = A.matriz[lin, col] - B.matriz[lin, col];
                    } // for int col
                return (C);
            } // try
            catch
            {
                return (null);
            } // catch

        } // Matriz operator -()

        /// <summary>
        /// MULTIPLICA DUAS MATRIZES [A] E [B], DESDE QUE [A] E [B] OBEDECAM A REGRA:
        /// O NUMERO DE COLUNAS DE [A] DEVE SER IGUAL AO NUMERO DE COLUNAS DE [B].
        /// </summary>
        /// <param name="A">MATRIZ MULTIPLICANDA [A].</param>
        /// <param name="B">MATRIZ MULTIPLICANDA [B].</param>
        /// <returns>RETORNA O PRODUTO DA MULTIPLICACAO DE [A] POR [B].</returns>
        public static Matriz operator *(Matriz A, Matriz B)
        {
            if (A.qtCol == B.qtLin)
            {
                Matriz C = Matriz.MulMatriz(A, B);
                return (C);
            }
            else
                return (null);
        } // operator *
        private double maxMatriz()
        {
            double max = this.matriz[0, 0];
            for (int lin = 0; lin < this.qtLin; lin++)
            {
                for (int col = 0; col < this.qtCol; col++)
                {
                    if (this.matriz[lin, col] > max)
                        max = this.matriz[lin, col];

                } // for int col
            } // for lin
            return (max);
        } // maxMatriz()

        /// <summary>
        /// DIVIDE QUALQUER MATRIZ [A] POR [B], DESDE QUE [A] E [B] OBEDECAM A REGRA DE
        /// MULTIPLICACAO DE MATRIZES: O NUMERO DE COLUNAS DE [A] DEVE SER IGUAL AO NU-
        /// MERO DE COLUNAS DE [B]. OU AINDA NUMERO DE LINHAS IGUAIS POR [A] E [B].
        /// </summary>
        /// <param name="A">Matriz a ser dividida.</param>
        /// <param name="B">Matriz divisora.</param>
        /// <returns>retorna o produto da divisao.</returns>
        public static Matriz operator /(Matriz v3, Matriz v2)
        {
            if (v3.qtCol == v2.qtCol)
            {


                aleatorizador = new Random(100);
                Matriz aux = new Matriz(v2.qtCol, v2.qtLin);
                aux.preencheMatriz(8);

                Matriz aux2 = new Matriz(v2.qtLin, v2.qtLin);
                aux2.preencheMatriz(6);

                Matriz invV2AuxPlusAux2plus = Matriz.MatrizInversa((v2 * aux) + aux2);


                Matriz T = ((v3 * aux) * invV2AuxPlusAux2plus);

                return (T);
            }
            else
                if (v2.qtLin == v3.qtLin)
                {
                    aleatorizador = new Random(100);
                    Matriz aux = new Matriz(v2.qtCol, v2.qtLin);
                    aux.preencheMatriz(8);

                    Matriz aux2 = new Matriz(v2.qtCol, v2.qtCol);
                    aux2.preencheMatriz(6);

                    Matriz invV2AuxPlusAux2plus = Matriz.MatrizInversa((aux * v2) + aux2);


                    Matriz T = ((aux * v3) * invV2AuxPlusAux2plus);

                    return (T);
                } // if
                else
                {

                    throw new Exception("Parametros incorretos para a divisao de matrizes");
                }//  else           
        }// operator /

        /// <summary>
        /// calcula a matriz inversa da matriz parametro.
        /// </summary>
        /// <param name="A">matriz a ter a inversa calculada.</param>
        /// <returns></returns>
        public static Matriz MatrizInversa(Matriz A)
        {
            if ((A.qtCol == 1) && (A.qtLin == 1))
            {
                Matriz M = new Matriz(1, 1);
                M.matriz[0, 0] = 1 / A.matriz[0, 0];
                return (M);
            } // if
            else
            {
                Matriz Adjt = A.Adjunta();
                Matriz M = new Matriz(A.qtLin, A.qtCol);
                int lin, col;
                double D = Matriz.determinante(A);
                for (lin = 0; lin < A.qtLin; lin++)
                    for (col = 0; col < A.qtCol; col++)
                        M.matriz[lin, col] = Adjt.matriz[lin, col] / D;
                return (M.Transposta());
            }
        } // MatrizInversa()


        /// <summary>
        /// calcula a matriz Transposta da matriz que chamou o metodo.
        /// </summary>
        /// <returns>retorna a matriz Transposta da matriz que chamou o método.</returns>
        private Matriz Adjunta()
        {
            int lin, col;
            Matriz Cofatores = new Matriz(this.qtLin, this.qtCol);
            for (lin = 0; lin < this.qtLin; lin++)
                for (col = 0; col < this.qtCol; col++)
                {
                    Cofatores.matriz[lin, col] = Matriz.determinante(this.ReduzMatriz(lin, col));
                    if (((lin + col) % 2) == 1)
                        Cofatores.matriz[lin, col] = -Cofatores.matriz[lin, col];
                } // for col
            return (Cofatores);
        } // Adjunta()

        /// <summary>
        /// calcula a matriz Transposta da matriz que chamou o metodo.
        /// </summary>
        /// <returns>retorna a matriz Transposta.</returns>
        private Matriz Transposta()
        {
            Matriz M = new Matriz(this.qtCol, this.qtLin);
            int lin, col;
            for (lin = 0; lin < this.qtLin; lin++)
                for (col = 0; col < this.qtCol; col++)
                    M.matriz[col, lin] = this.matriz[lin, col];
            return (M);
        } // Transposta()


        /// <summary>
        /// calcula o determinante  de uma matriz.
        /// </summary>
        /// <param name="M">Matriz a ter o determinante calculado.</param>
        /// <returns></returns>
        public static double determinante(Matriz M)
        {
            if (M.qtLin == 2)
            {
                double d = M.matriz[0, 0] * M.matriz[1, 1] - M.matriz[0, 1] * M.matriz[1, 0];
                return (d);
            } // if M.qtLin
            else
                if (M.qtLin == 3)
                {
                    double d = +M.matriz[0, 0] * M.matriz[1, 1] * M.matriz[2, 2]
                               + M.matriz[0, 1] * M.matriz[1, 2] * M.matriz[2, 0]
                               + M.matriz[0, 2] * M.matriz[1, 0] * M.matriz[2, 1]
                               - M.matriz[0, 2] * M.matriz[1, 1] * M.matriz[2, 0]
                               - M.matriz[0, 0] * M.matriz[1, 2] * M.matriz[2, 1]
                               - M.matriz[0, 1] * M.matriz[1, 0] * M.matriz[2, 2];
                    return (d);
                }
                else
                    if (M.qtLin == 1)
                        return (M.matriz[0, 0]);
                    else
                        if (M.qtLin > 3)
                        {
                            double d = 0.0;
                            double sinal = 1.0;

                            for (int col = 0; col < M.qtCol; col++)
                            {
                                if ((col % 2) == 0)
                                    sinal = 1.0;
                                else sinal = -1.0;
                                Matriz N = M.ReduzMatriz(0, col);
                                d += sinal * determinante(N);
                            } // for col
                            return (d);
                        } // if M.qtLin>3
            return (0.0);
        } // double determinante()


        /// <summary>
        /// reduz a matriz que chamou o método,
        /// retirando a linha e coluna especificadas.
        /// </summary>
        /// <param name="linCorte">linha a ser retirada.</param>
        /// <param name="colCorte">coluna a ser retirada.</param>
        /// <returns></returns>
        private Matriz ReduzMatriz(int linCorte, int colCorte)
        {
            int lin, col;
            int linCount, colCount;
            Matriz M = new Matriz(this.qtLin - 1, this.qtCol - 1);
            linCount = 0;
            for (lin = 0; lin < this.qtLin; lin++)
            {
                colCount = 0;
                if (lin != linCorte)
                {
                    for (col = 0; col < this.qtCol; col++)
                        if (col != colCorte)
                        {
                            M.matriz[linCount, colCount] = this.matriz[lin, col];
                            colCount++;
                        } // if col!=colCorte
                    linCount++;
                } // if lin!=colCorte
            } // for lin
            return (M);
        } // ReduzMatriz()

        /// <summary>
        /// preenche a matriz com dados aleatorios. 
        /// </summary>
        /// <param name="intervalo">intervalo somada cumulativamente.</param>
        public void preencheMatriz(double intervalo)
        {
            int lin, col;
            aleatorizador = new Random(Matriz.seedRandom);
            for (col = 0; col < this.qtCol; col++)
                for (lin = 0; lin < this.qtLin; lin++)
                {
                    this.matriz[lin, col] = aleatorizador.Next() * 20 + 1;

                }
        } // void preencheMatriz()
        /// <summary>
        /// transforma uma matriz para uma representação em String. Sobrescreve 
        /// o método ToString(), para que possa representar o objeto Matriz em
        /// operações de escrita em tela.
        /// </summary>
        /// <returns>retorna uma String representação da matriz.</returns>
        public override string ToString()
        {
            String s = "";
            for (int lin = 0; lin < this.qtLin; lin++)
            {
                s = s + "[";
                for (int col = 0; col < this.qtCol; col++)
                {
                    s = s + " [" + (Math.Round(this.matriz[lin, col])).ToString() + "]";
                } // for int col
                s = s + "]";
            } // for int lin
            return (s);
        } // ToSTring()

        /// <summary>
        /// ordena a matriz se lin=1 ou col=1.
        /// </summary>
        public void Sort()
        {
            List<double> sortList = new List<double>();
            int lin, col;

            if (this.qtCol == 1)
            {

                for (lin = 0; lin < this.qtLin; lin++)
                {
                    sortList.Add(this.matriz[lin, 0]);
                } // for lin
                sortList.Sort();
                for (lin = 0; lin < this.qtLin; lin++)
                {
                    this.matriz[lin, 0] = sortList[lin];
                } // for lin

            } // if
            else
                if (this.qtLin == 1)
                {
                    for (col = 0; col < this.qtCol; col++)
                        sortList.Add(this.matriz[0, col]);
                    sortList.Sort();
                    for (col = 0; col < this.qtCol; col++)
                        this.matriz[0, col] = sortList[col];
                } // if 
        } // Sort().

        /// <summary>
        /// tranforma um vetor 3D em uma matriz.
        /// </summary>
        /// <param name="v">vetor 3D a ser transformado em matriz.</param>
        /// <returns>retorna uma matriz [3,1] representando o vetor 3D.</returns>
        public static Matriz DeVetorParaMatriz(vetor3 v)
        {
            Matriz mr = new Matriz(3, 1);
            mr.setElemento(0, 0, v.X);
            mr.setElemento(1, 0, v.Y);
            mr.setElemento(2, 0, v.Z);
            return mr;
        } // DeVetorParaMatriz()

        /// <summary>
        /// transforma uma matriz [3,1] em um vetor 3D.
        /// </summary>
        /// <param name="m">matriz a ser convertida em vetor 3D.</param>
        /// <returns>retorna um vetor 3D de uma matriz [3,1].</returns>
        public static vetor3 DeMatrizParaVetor(Matriz m)
        {
            vetor3 a = new vetor3();
            a.X = m.getElemento(0, 0);
            a.Y = m.getElemento(1, 0);
            a.Z = m.getElemento(2, 0);
            return a;
        } // DeMatrizParaVetor()
    } // class Matriz
    //********************************************************************************************************************

    /// <summary>
    /// classe de vetores 2D.
    /// </summary>
    public class vetor2
    {
        /// <summary>
        /// coordenada X do vetor 2D.
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// coordenada Y do vetor 2D.
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// cor associada ao vetor 2D.
        /// </summary>
        public Color cor { get; set; }

        /// <summary>
        /// operador de multiplicação de um vetor 2D por um número.
        /// </summary>
        /// <param name="n">número multiplicativo.</param>
        /// <param name="v">vetor 2D a ser multiplicado.</param>
        /// <returns>retorna o vetor multiplicado pelo número parâmetro.</returns>
        public static vetor2 operator *(double n, vetor2 v)
        {
            return (new vetor2(v.X * n, v.Y * n));
        }

        /// <summary>
        /// operação de adição entre dois vetores 2D.
        /// </summary>
        /// <param name="v1">vetor 2D para adição.</param>
        /// <param name="v2">vetor 2D para adição.</param>
        /// <returns>retorna um vetor 2D que é a soma dos vetores parâmetros.</returns>
        public static vetor2 operator +(vetor2 v1, vetor2 v2)
        {
            return (new vetor2(v1.X + v2.X, v1.Y + v2.Y));
        }

        /// <summary>
        /// operação de subtração entre dois vetores 2D.
        /// </summary>
        /// <param name="v1">vetor2D para subtração.</param>
        /// <param name="v2">vetor2D para subtração.</param>
        /// <returns>retorna um vetor resultante da subtração do primeiro vetor 2D pelo segundo.</returns>
        public static vetor2 operator -(vetor2 v1, vetor2 v2)
        {
            return (new vetor2(v1.X - v2.X, v1.Y - v2.Y));
        }

        public vetor2(double x, double y, Color _cor)
        {
            this.X = x;
            this.Y = y;
            this.cor = _cor;

        }

        public vetor2(double x, double y)
        {
            this.X = x;
            this.Y = y;
            this.cor = Color.White;
        }

        public void perspectivaIsometrica(vetor3 v)
        {
            this.X = v.X + v.Z / 2;
            this.Y = v.Y + v.Z / 2;
            this.cor = v.cor;
        }
        /// <summary>
        /// faz a normalizacao de um vetor 2D (magnitude = 1.0F);
        /// </summary>
        /// <param name="p">vetor 2D a ser normalizado.</param>
        /// <returns>retornao o vetor 2D normalizado.</returns>
        public static vetor2 normaliza(vetor2 p)
        {
            double d = (double)Math.Sqrt(p.X * p.X + p.Y * p.Y);
            return (new vetor2(p.X / d, p.Y / d, p.cor));

        }
        /// <summary>
        /// calcula o quadrante do vetor 2D (conceito de trigonometria).
        /// </summary>
        /// <returns>retorna 1,2,3,ou 4, sendo os quadrantes indo no sentido anti-horário.</returns>
        public int quadranteDoVetor2D()
        {
            if ((this.X >= 0) && (this.Y >= 0))
                return 1;
            if ((this.X < 0) && (this.Y > 0))
                return 2;
            if ((this.X < 0) && (this.Y < 0))
                return 3;
            if ((this.X > 0) && (this.Y < 0))
                return 4;
            return -1;
        } // quadranteDoVetor2D()


        /// <summary>
        /// determina se o vetor currente e o vetor parâmetro são iguais,
        /// </summary>
        /// <param name="v">vetor 2D parâmetro.</param>
        /// <returns>[true] se os vetores são iguais, [false] se não são.</returns>
        public bool igualVetor2(vetor2 v)
        {
            if ((Math.Abs(this.X - v.X) < 0.001) && (Math.Abs(this.Y - v.Y) < 0.001))
                return true;
            return false;
        } // igualVetor2()


        public static double operator *(vetor2 v1, vetor2 v2)
        {
            return (v1.X * v2.X+v1.Y * v2.Y);
        } // operator *()
    }
    //****************************************************************************************************************************
    /// <summary>
    /// classe de vetores 3D, conversíveis à matrizes [1,3].
    /// </summary>
    public class vetor3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public enum planoRotacao { XY, YZ, XZ }
        public enum trocaCordenadas { X, Y, Z }
        public enum tipoAngulo { Absoluto, Relativo }
        public Color cor { get; set; }

        /// <summary>
        /// inicializa um vetor 3D, preenchendo com 0.0 nas coordenadas.
        /// </summary>
        public vetor3()
        {
            this.X = 0.0;
            this.Y = 0.0;
            this.Z = 0.0;
        }
        
        /// <summary>
        /// inicializa um vetor 3D, com coordenadas dadas.
        /// </summary>
        /// <param name="x">parâmetro para a coordenada X.</param>
        /// <param name="y">parâmetro para a coordenada Y.</param>
        /// <param name="z">parâmetro para a coordenada Z.</param>
        public vetor3(double x, double y, double z)
        {

            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        /// <summary>
        /// inicializa um vetor 3D, com coordenadas dadas e cor dada.
        /// </summary>
        /// <param name="x">parâmetro para a coordenada X.</param>
        /// <param name="y">parâmetro para a coordenada Y.</param>
        /// <param name="z">parâmetro para a coordenada Z.</param>
        /// <param name="c">parâmetro para a cor do vetor 3D.</param>
        public vetor3(double x, double y, double z, Color c)
        {

            this.X = x;
            this.Y = y;
            this.Z = z;
            this.cor = c;
        }

        /// <summary>
        /// inicializa um vetor 3D, fazendo uma cópia do vetor 3D parâmetro.
        /// </summary>
        /// <param name="v">vetor 3D a ser copiado.</param>
        public vetor3(vetor3 v)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;
            this.cor = v.cor;
        }

        /// <summary>
        /// faz uma cópia do vetor 3D, do vetor 3D que chamou este método.
        /// </summary>
        /// <returns></returns>
        public vetor3 Clone()
        {
            return (new vetor3(this.X, this.Y, this.Z, this.cor));
        }

        /// <summary>
        /// rotaciona um vetor nos planos XY (com acrescimo do angulo nOmega)
        /// e Z (com acrescimo do angulo nTeta). Os ÂNGULOS DEVEM ESTAR EM RADIANOS..
        /// </summary>
        /// <param name="nOmega">angulo de acrescimo, atuando no plano XZ.</param>
        /// <param name="nTeta">angulo de acrescimo, atuando no eixo  Y.</param>
        /// <returns>retorna o vetor [v] rotacionado.</returns>
        public void rotacionaVetor(double nOmega, double nTeta)
        {
            double omega = this.encontraAnguloOmega();
            double teta = this.encontraAnguloTeta();
            double r = this.raio();
            this.X = r * Math.Cos(omega + nOmega) * Math.Sin(teta + nTeta);
            this.Y = r * Math.Sin(omega + nOmega) * Math.Sin(teta + nTeta);
            this.Z = r * Math.Cos(teta + nTeta);
        }// rotacionaVetor()
        
        /// <summary>
        /// rotaciona o vetor 3D, conforme o plano (XY,YZ,ou XZ).
        /// Os ângulos devem estar em radianos.
        /// </summary>
        /// <param name="omega">ângulo de coordenadas esféricas.</param>
        /// <param name="teta">ângulo de coordenadas esféricas.</param>
        /// <param name="plano">plano a ser rotacionado.</param>
        public void rotacionaVetor(double omega, double teta, planoRotacao plano)
        {
            switch (plano)
            {
                case planoRotacao.XY:
                    rotacionaVetor(omega, teta);
                    break;
                case planoRotacao.XZ:
                    trocaCoordenadas(trocaCordenadas.Y, trocaCordenadas.Z);
                    rotacionaVetor(omega, teta);
                    trocaCoordenadas(trocaCordenadas.Y, trocaCordenadas.Z);
                    break;
                case planoRotacao.YZ:
                    trocaCoordenadas(trocaCordenadas.X, trocaCordenadas.Z);
                    rotacionaVetor(omega, teta);
                    trocaCoordenadas(trocaCordenadas.X, trocaCordenadas.Z);
                    break;
            } // switch plano
        } // rotacionaVetor

        
        /// rotaciona o vetor simultaneamente no plano XZ,YZ e XY, nesta ordem, com acréscimos de ângulos em Graus.
        /// Calculos:
        ///  x=r*cos(tetaIni+teta)
        ///  x=r*(cos(tetaIni)*cos(teta)-sen(tetaIni)*sen(teta))
        ///  
        ///  x=xini*cos(teta)-yini*sen(teta)
        ///
        ///  y=r*sen(tetaIni+teta)
        ///  y=r*(sen(tetaIni)*cos(teta)+sen(teta)*cos(tetaIni))
        ///  
        ///  y=yini*cos(teta)+xini*sen(teta)
        ///  y=xini*sen(teta)+yini*cos(teta)
        ///
        /// 
        /// Matriz de rotação (plano XY):
        /// | cos(teta)    sen(teta) 0|
        /// |-sen(teta)    cos(teta) 0|
        /// |     0            0     1|
        /// <summary>
        /// rotaciona um vetor, intercalando 3 matrizes de rotação, para rotacionar os três planos (XY,YZ e XZ).
        /// </summary>
        /// <param name="anguloRotacaoPlanoXZEmGraus">ângulo para o plano XZ.</param>
        /// <param name="anguloRotacaoPlanoYZEmGraus">ângulo para o plano YZ.</param>
        /// <param name="anguloRotacaoPlanoXYEmGraus">ângulo para o plano XY.</param>
        /// <param name="isAnguloAbsoluto">se [true] os ângulos do vetor são substituidos pelos ângulos parâmetros,
        /// se [false] os ângulos do vetor são acrescidos com os ângulos parâmetros.</param>
        public void rotacionaVetor(double anguloRotacaoPlanoXZEmGraus,
                                   double anguloRotacaoPlanoYZEmGraus,
                                   double anguloRotacaoPlanoXYEmGraus)
        {

            double omegaXY=angulos.toRadianos(anguloRotacaoPlanoXYEmGraus);
            double omegaXZ=angulos.toRadianos(anguloRotacaoPlanoXZEmGraus);
            double omegaYZ=angulos.toRadianos(anguloRotacaoPlanoYZEmGraus);

            // converte o vetor currente para uma matriz.
            Matriz vetor = new Matriz(1, 3, X, Y, Z);
            // matriz de rotação para o plano XY.
            Matriz rotXY = new Matriz(3, 3, Math.Cos(omegaXY), Math.Sin(omegaXY), 0,
                                           -Math.Sin(omegaXY), Math.Cos(omegaXY), 0,
                                            0, 0, 1);
            // matriz de rotação para o plano XZ.
            Matriz rotXZ = new Matriz(3, 3, Math.Cos(omegaXZ), 0, Math.Sin(omegaXZ),
                                            0, 1, 0,
                                           -Math.Sin(omegaXZ), 0, Math.Cos(omegaXZ));
            // matriz de rotação para o plano YZ.
            Matriz rotYZ = new Matriz(3, 3, 1, 0, 0,
                                            0, Math.Cos(omegaYZ), Math.Sin(omegaYZ),
                                            0, -Math.Sin(omegaYZ), Math.Cos(omegaYZ));

            // multiplicação da matriz-vetor pelas matrizes de rotação.
            Matriz vetorFinal = vetor * rotXY * rotXZ * rotYZ;

            // extrai o vetor currente da matriz-vetor.
            this.X = vetorFinal.getElemento(0, 0);
            this.Y = vetorFinal.getElemento(0, 1);
            this.Z = vetorFinal.getElemento(0, 2);
        } // rotacionaVetor()


        /// <summary>
        /// rotaciona o vetor, em ângulos absolutos (sem acréscimos de ângulos).
        /// Encontra os ângulos iniciais, e depois rotaciona em sentido contrário,
        /// acrescentando em seguida os ângulos parâmetros.
        /// </summary>
        /// <param name="anguloRotPlanoXYEmGraus">ângulo absoluto para o plano XY, em Graus.</param>
        /// <param name="anguloRotPlanoYZEmGraus">ângulo absoluto para o plano YZ, em Graus.</param>
        /// <param name="anguloRotPlanoXZEmGraus">ângulo absoluto para o plano XZ, em Graus.</param>
        public void rotacionaVetorAnguloAbsoluto(double anguloRotPlanoXYEmGraus,
                                                 double anguloRotPlanoYZEmGraus,
                                                 double anguloRotPlanoXZEmGraus)
        {
            double anguloOmegaXYinicial = angulos.toGraus(this.encontraAnguloOmega(planoRotacao.XY));
            double anguloOmegaXZinicial = angulos.toGraus(this.encontraAnguloOmega(planoRotacao.XZ));
            double anguloOmegaYZinicial = angulos.toGraus(this.encontraAnguloOmega(planoRotacao.YZ));
            this.rotacionaVetor(-anguloOmegaXZinicial + anguloRotPlanoXZEmGraus,
                                -anguloOmegaYZinicial + anguloRotPlanoYZEmGraus,
                                -anguloOmegaXYinicial + anguloRotPlanoXYEmGraus);
         } // rotacionaVetorComPlanosAnguloAbsoluto()

        public void rotacionaVetorAnguloAbsolutoOuRelativo(tipoAngulo typeAngle, double anguloXZEmGraus,
            double anguloYZEmGraus, double anguloXYEmGraus)
        {
            if (typeAngle == tipoAngulo.Relativo)
                this.rotacionaVetor(anguloXZEmGraus, anguloYZEmGraus, anguloXYEmGraus);
            if (typeAngle == tipoAngulo.Absoluto)
                this.rotacionaVetorAnguloAbsoluto(anguloXYEmGraus, anguloYZEmGraus, anguloXZEmGraus);
        } // rotacionaVetorAnguloAbsolutoOuRelativo()
        /// <summary>
        /// modifica o raio do deste vetor3.
        /// </summary>
        /// <param name="nraio">novo raio.</param>
        public void modificaRaio(double nraio)
        {
            double omega = this.encontraAnguloOmega();
            double teta = this.encontraAnguloTeta();
            this.X = nraio * Math.Cos(omega) * Math.Sin(teta);
            this.Z = nraio * Math.Sin(omega) * Math.Sin(teta);
            this.Y = nraio * Math.Cos(teta);
         } // modificaRaio()
        /// <summary>
        /// retorna 0 se Abs([n]) Menor que 0.0001, retorna o próprio [n] outro caso.
        /// </summary>
        /// <param name="n">número a ser arredondado o 0.</param>
        /// <returns></returns>
        private double arredonda(double n)
        {
            if (Math.Abs(n) < 0.0001)
                return 0.0;
            return n;
        } // arredonda()
        /// <summary>
        /// encontra o ângulo omega (coordenadas esféricas), para um determinado plano (XY,YZ, ou XZ).
        /// </summary>
        /// <param name="plano">plano (XY,YZ, ou XZ) a ter o ângulo omega calculado.</param>
        /// <returns>retorna o ângulo omega para determinado plano. Por default, retorna o ângulo omega do plano XY.</returns>
        public double encontraAnguloOmega(vetor3.planoRotacao plano)
        {
            switch (plano)
            {
                case vetor3.planoRotacao.XY:
                    return encontraAnguloOmegaPlanoXY();
                case vetor3.planoRotacao.XZ:
                    return encontraAnguloOmegaPlanoXZ();
                case vetor3.planoRotacao.YZ:
                    return encontraAnguloOmegaPlanoYZ();
                default:
                    return encontraAnguloOmegaPlanoXY();
            } // switch
        } // encontraAnguloOmega()

        /// <summary>
        /// encontra o ângulo teta (coordenadas esféricas) para um determinado plano (XY,YZ,ou XZ).
        /// </summary>
        /// <param name="plano">plano a obter o ângulo teta.</param>
        /// <returns>retorna o ângulo teta. Por default, encontra o ângulo teta do plano XY.</returns>
        public double encontraAnguloTeta(vetor3.planoRotacao plano)
        {
            switch (plano)
            {
                case planoRotacao.XY:
                    return encontraAnguloTetaPlanoXY();
                case planoRotacao.XZ:
                    return encontraAnguloTetaPlanoXZ();
                case planoRotacao.YZ:
                    return encontraAnguloTetaPlanoYZ();
                default:
                    return encontraAnguloTetaPlanoXY();
            } // switcch
        } // encontraAnguloTeta()

        /// <summary>
        /// calcula a matriz de cossenos-diretores para mudança 
        /// de base de uma base antiga com eixos (i1,j1,k1)
        /// para uma base com eixos (i0,j0,k0).
        /// </summary>
        /// <param name="i0">antigo eixo X.</param>
        /// <param name="j0">antigo eixo Y.</param>
        /// <param name="k0">antigo eixo Z.</param>
        /// <param name="i1">novo eixo X.</param>
        /// <param name="j1">novo eixo Y.</param>
        /// <param name="k1">novo eixo Z.</param>
        /// <returns>retorna a matriz de mudança de base do sistema de coordenadas com base (i1,j1,k1) para
        /// o sistema de coordenadas com base (i0,j0,k0).</returns>
        public static double[,] matrizMudancaDeBase(
            vetor3 i0, vetor3 j0, vetor3 k0,
            vetor3 i1, vetor3 j1, vetor3 k1)
        {
            double[,] matrizMudanca = new double[3, 3];
            i0.normaliza();
            j0.normaliza();
            k0.normaliza();

            i1.normaliza();
            j1.normaliza();
            k1.normaliza();

            double Ax = Math.Acos(i0 * i1);
            double Ay = Math.Acos(i0 * j1);
            double Az = Math.Acos(i0 * k1);

            double Bx = Math.Acos(j0 * i1);
            double By = Math.Acos(j0 * j1);
            double Bz = Math.Acos(j0 * k1);

            double Cx = Math.Acos(k0 * i1);
            double Cy = Math.Acos(k0 * j1);
            double Cz = Math.Acos(k0 * k1);

            matrizMudanca[0, 0] = Math.Cos(Ax);
            matrizMudanca[1, 0] = Math.Cos(Ay);
            matrizMudanca[2, 0] = Math.Cos(Az);
            matrizMudanca[0, 1] = Math.Cos(Bx);
            matrizMudanca[1, 1] = Math.Cos(By);
            matrizMudanca[2, 1] = Math.Cos(Bz);
            matrizMudanca[0, 2] = Math.Cos(Cx);
            matrizMudanca[1, 2] = Math.Cos(Cy);
            matrizMudanca[2, 2] = Math.Cos(Cz);

            return matrizMudanca;
        } // matrizMudancaDeBase()
        
        /// <summary>
        /// retorna o ângulo da cordenada de altura (coordenada Y).
        /// </summary>
        /// <returns></returns>
        public double encontraAnguloTeta()
        {
            return Math.Atan2(Math.Sqrt(X*X+Y*Y),Z);
        } // encontraAnguloTeta()


        /// <summary>
        /// retorna o ângulo do plano XZ em relação ao eixo X.
        /// </summary>
        /// <returns></returns>
        public double encontraAnguloOmega()
        {
            return Math.Atan2(this.Y, this.X);
        } // encontraAnguloOmega()

        /// <summary>
        /// retorna o raio do vetor3 currente.
        /// </summary>
        /// <returns></returns>
        public double raio()
        {
            return ((Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z)));
        } // raio()

        /// <summary>
        /// retorna o módulo do vetor3 currente.
        /// </summary>
        /// <returns></returns>
        public double modulo()
        {
            return raio();
        } // modulo()

        /// <summary>
        /// normaliza o vetor3 currente.
        /// </summary>
        public void normaliza()
        {
            double mod = this.modulo();
            this.X /= mod;
            this.Y /= mod;
            this.Z /= mod;
        } // normaliza()

        /// <summary>
        /// troca coordenadas, obedecendo uma numeração de variáveis.
        /// se nv1=0 e nv2=1, e vice-versa: troca X por Y.
        /// se nv1=0 e nv2=2, e vice-versa: troca X por Z.
        /// se nv1=1 e nv2=2, e vice-versa: troca Y por Z.
        /// </summary>
        /// <param name="nv1">variável simbólica 1.</param>
        /// <param name="nv2">variável simbólica 2.</param>
        public void trocaCoordenadas(vetor3.trocaCordenadas n1, vetor3.trocaCordenadas n2)
        {
            double nx=this.X;
            double ny= this.Y;
            double nz= this.Z;
            if (n1 == n2)
                return;
            if (((n1 == trocaCordenadas.X) && (n2 == trocaCordenadas.Y)) || ((n1 == trocaCordenadas.Y) && (n2 == trocaCordenadas.X)))
                this.swap(ref nx, ref ny);
            if (((n1 == trocaCordenadas.X) && (n2 == trocaCordenadas.Z)) || ((n1 == trocaCordenadas.Z) && (n2 == trocaCordenadas.X)))
                this.swap(ref nx, ref nz);
            if (((n1 == trocaCordenadas.Y) && (n2 == trocaCordenadas.Z)) || ((n1 == trocaCordenadas.Z) && (n2 == trocaCordenadas.Y)))
                this.swap(ref ny, ref nz);
            this.X = nx;
            this.Y = ny;
            this.Z = nz;
        }  // trocaCoordenadas()
        
        /// <summary>
        /// operação de adição entre dois vetores 3D.
        /// </summary>
        /// <param name="v1">vetor 3D parâmetro.</param>
        /// <param name="v2">vetor 3D parâmetro.</param>
        /// <returns></returns>
        public static vetor3 operator +(vetor3 v1, vetor3 v2)
        {
            return new vetor3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        } // operator +()
        
        /// <summary>
        /// operação de subtração entre dois vetores 3D.
        /// </summary>
        /// <param name="v1">vetor 3D parâmetro.</param>
        /// <param name="v2">vetor 3D parâmetro.</param>
        /// <returns></returns>
        public static vetor3 operator -(vetor3 v1, vetor3 v2)
        {
            return new vetor3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        } // operator -()

        /// <summary>
        /// troca o valor de dois números.
        /// </summary>
        /// <param name="num1">número 1 a ser trocado.</param>
        /// <param name="num2">número 2 a ser trocado.</param>
        private void swap(ref double num1, ref double num2)
        {
            double tmp = num1;
            num1 = num2;
            num2 = tmp;
        }
        /// <summary>
        /// calcula o produto escalar entre dois vetores.
        /// </summary>
        /// <param name="v1">vetor 1 a gerar o produto escalar.</param>
        /// <param name="v2">vetor 2 a gerar o produto escalar.</param>
        /// <returns>retorna o produto escalar, calculado pela
        ///  multiplicação entre as respectivas coordenadas (x,y,z).</returns>
        public static double operator *(vetor3 v1, vetor3 v2)
        {
            return (v1.X * v2.X+ v1.Y * v2.Y+ v1.Z * v2.Z);
        } // operator *()

        
        /// <summary>
        /// calcula o produto entre um número e um vetor 3D.
        /// </summary>
        /// <param name="n">número a ser multiplicado.</param>
        /// <param name="v1">vetor 3D a ser multiplicado.</param>
        /// <returns></returns>
        public static vetor3 operator *(double n, vetor3 v1)
        {
            return new vetor3(n * v1.X, n * v1.Y, n * v1.Z);
        } // operator *()

        /// <summary>
        /// calcula o produto vetorial de dois vetores dados.
        /// </summary>
        /// <param name="v1">vetor 1 a gerar o produto vetorial.</param>
        /// <param name="v2">vetor 2 a gerar o produto vetorial.</param>
        /// <returns>retorna o produto vetorial, determinada pelo determinante
        /// dos dois vetores, mais as cordenadas literais (x,y,z).</returns>
        public static vetor3 operator &(vetor3 v1, vetor3 v2)
        {
            vetor3 vr = new vetor3();
            vr.X = v1.Y * v2.Z - v1.Z * v2.Y;
            vr.Y = v1.Z * v2.X - v1.X * v2.Z;
            vr.Z = v1.X * v2.Y - v1.Y * v2.X;
            return (vr);

        } // produtoVetorial()
        /// <summary>
        /// retorna um vetor com todas as coordenadas igual a zero.
        /// </summary>
        /// <returns>retorna o vetor 3D zero.</returns>
        public static vetor3 zero()
        {
            return new vetor3(0.0, 0.0, 0.0);
       }

        /// <summary>
        /// transformação isométrica simples, trasladando-se a coordenada 3D z para cada cordenada 2D x e y.
        /// </summary>
        /// <param name="v">vetor 3D a ser transformado em perspectiva isométrica.</param>
        /// <param name="fatorIsometrico">índice de divisão sobre a cordenada que é projetada nos eixos 2D.</param>
        /// <returns>retorna o vetor 2D transformado isometricamente.</returns>
        public static vetor2 transformacaoPerspectivaIsometrica(vetor3 v, double fatorIsometrico)
        {
            vetor2 v2 = new vetor2(v.X + v.Z / fatorIsometrico, v.Y + v.Z / fatorIsometrico);
            v2.cor = v.cor;
            return v2;
        } // transformacaoPerspectivaIsometrica()

        /// <summary>
        /// transformação geométrica simples, dividindo as coordenadas [x] e [y] pela
        /// coordenda 3D [z], aumentada e multiplicada esta por um [fatorGeometrico].
        /// </summary>
        /// <param name="v"></param>
        /// <param name="fatorGeometrico"></param>
        /// <returns></returns>
        public static vetor2 transformacaoPerspectivaGeometrica(vetor3 v, double fatorGeometrico)
        {
            vetor2 v2 = new vetor2((fatorGeometrico + 20.0) * v.X / (v.Z + fatorGeometrico),
                                   (fatorGeometrico + 20.0) * v.Y / (v.Z + fatorGeometrico));
            return v2;
        } // transformacaoPerspectivaGeometrica()

        /// <summary>
        /// realiza a transformação isométrica simples, no vetor 3D que chamou este método.
        /// </summary>
        /// <param name="fatorIsometrico">fator de divisão para a coordenada de profundidade.</param>
        /// <returns>retorna o vetor 2D transformado ismetricamente.</returns>
        public vetor2 transformacaoPerspecctivaIsometrica(double fatorIsometrico)
        {
            return (vetor3.transformacaoPerspectivaIsometrica(this, fatorIsometrico));
        }

        /// <summary>
        /// multiplica o vetor currente por uma base das pelos eixos vetores de entrada.
        /// </summary>
        /// <param name="i">eixo i.</param>
        /// <param name="j">eixo j.</param>
        /// <param name="k">eixo k.</param>
        public void multiplicaPorUmaBase(vetor3 i, vetor3 j, vetor3 k)
        {
            vetor3 v = new vetor3(this);
            this.X = v.X * i.X + v.Y * j.X + v.Z * k.X;
            this.Y = v.X * i.Y + v.Y * j.Y + v.Z * k.Y;
            this.Z = v.X * i.Z + v.Y * j.Z + v.Z * k.Z;
        } // multiplicaPorUmaBase()

        
        /// <summary>
        /// converte um angulo em graus para radiano.
        /// </summary>
        /// <param name="anguloGraus">angulo em graus a ser convertido.</param>
        /// <returns>retorna o angulo convertido em radianos.</returns>
        public static double toRadianos(double anguloGraus)
        {
            return (anguloGraus * ((double)Math.PI / 180.0F));
        }
        /// <summary>
        /// converte um angulo em radianos para graus
        /// </summary>
        /// <param name="angleRadianos">angulo em radianos a ser convertido.</param>
        /// <returns>retorna o angulo convertido em graus</returns>
        public static double toGraus(double angleRadianos)
        {
            return (angleRadianos * 180.0F / (double)Math.PI);
        }
        
        /// <summary>
        /// calcula o ângulo em radianos entre dois vetores.
        /// </summary>
        /// <param name="v1">vetor 1 para o cálculo.</param>
        /// <param name="v2">vetor 2 para o cálculo.</param>
        /// <returns>retorna o ângulo entre os dois vetores.</returns>
        public static double calcAnguloEntreVetores(vetor3 v1, vetor3 v2)
        {
            double angulo = Math.Acos((1 / (v1.modulo() * v2.modulo())) * (v1 * v2));
            return angulo;
            
        } // calcAnguloEntreVetores()

        /// <summary>
        /// calcula o raio (magnitude) do vetor 3D [v].
        /// </summary>
        /// <param name="v">vetor 3D de entrada.</param>
        /// <returns>retorna a magnitude do vetor 3D de entrada.</returns>
        public static double raio(vetor3 v)
        {
            return (Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z));
        } // raio()

        /// <summary>
        /// converte o tipo [Matriz] para o tipo [vetor3], se houver
        /// compatibilizaçao de linhas e colunas da matriz de entrada (dimensões [3,1]).
        /// Se não houver compatibilização de dimensões, lança uma exceção.
        /// </summary>
        /// <param name="mi">[Matriz] a ser convertida.</param>
        /// <returns>o vetor3 resulante da conversão.</returns>
        public static vetor3 deMatrizTovetor3(Matriz mi)
        {
            // se as dimensões não são compatíveis, lança uma exceção.
            if ((mi.qtLin != 3) && (mi.qtCol != 1))
                throw new Exception("dimensões da matriz: " + mi.ToString() + "de entrada não são compatíveis com as dimensões de vetor3");
            vetor3 v = new vetor3(mi.getElemento(0, 0), mi.getElemento(1, 0), mi.getElemento(2, 0));
            return v;
        } // tovetor3()

        /// <summary>
        /// encontra um ângulo teta (coordenada esférica) para o plano XZ.
        /// </summary>
        /// <returns></returns>
        private double encontraAnguloTetaPlanoXZ()
        {
            return Math.Atan2(Math.Sqrt(X * X + Z * Z), Y);
        } // encontraAnguloTetaPlanoXZ

        /// <summary>
        /// encontra um ângulo omega (coordenada esférica) para o plano XZ.
        /// </summary>
        /// <returns></returns>
        private double encontraAnguloOmegaPlanoXZ()
        {
            return Math.Atan2(Z, X);
        } // encontraAnguloOmegaPlanoXZ()

        /// <summary>
        /// encontra um ângulo teta (coordenada esférica) para o plano YZ.
        /// </summary>
        /// <returns></returns>
        private double encontraAnguloTetaPlanoYZ()
        {
            return Math.Atan2(Math.Sqrt(Y * Y + Z * Z), X);
        } // encontraAnguloTetaPlanoYZ()

        /// <summary>
        /// encontra um ângulo omega (coordenada esférica) para o plano YZ.
        /// </summary>
        /// <returns></returns>
        private double encontraAnguloOmegaPlanoYZ()
        {
            return Math.Atan2(Z, Y);
        } // encontraAnguloOmegaPlanoYZ()

        /// <summary>
        /// encontra um ângulo teta (coordenada esférica) para o plano XY.
        /// </summary>
        /// <returns></returns>
        private double encontraAnguloTetaPlanoXY()
        {
            return encontraAnguloTeta();

        } //encontraAnguloTetaPlanoXY

        /// <summary>
        /// encontra um ângulo omega (coordenada esférica) para o plano XY.
        /// </summary>
        /// <returns></returns>
        private double encontraAnguloOmegaPlanoXY()
        {
            return encontraAnguloOmega();
        } // encontraAnguloOmegaPlanoXY()

        public override string ToString()
        {
            double xx = this.X;
            double yy = this.Y;
            double zz = this.Z;
            if (Math.Abs(this.X) < 0.0001)
                xx = Math.Round(this.X);
            if (Math.Abs(this.Y) < 0.0001)
                yy = Math.Round(this.Y);
            if (Math.Abs(this.Z) < 0.0001)
                zz = Math.Round(this.Z);
            return ("(" + xx + " , " + yy + " , " + zz + ")");
        }

        /// <summary>
        /// clona a matriz de entrada.
        /// </summary>
        /// <param name="matrix">matriz a ser clonada.</param>
        /// <returns>retorna um clone da matriz clonada.</returns>
        public static vetor3[, ,] cloneMatriz(vetor3[, ,] matrix)
        {
            vetor3[, ,] mcln = new vetor3[matrix.GetLength(0), matrix.GetLength(1), matrix.GetLength(2)];
            int x, y, z;
            for (z = 0; z < matrix.GetLength(2); z++)
                for (y = 0; y < matrix.GetLength(1); y++)
                    for (x = 0; x < matrix.GetLength(0); x++)
                    {
                        mcln[x, y, z] = new vetor3(matrix[x, y, z]);
                    } // for x
            return mcln;
        } // cloneMatriz()

    } // class vetor3

    public class angulos
    {
        /// <summary>
        /// converte um ângulo em graus para um ângulo em radianos.
        /// </summary>
        /// <param name="anguloEmGraus">ângulo em graus a ser convertido.</param>
        /// <returns>retorna um ângulo em radianos.</returns>
        public static double toRadianos(double anguloEmGraus)
        {
            return (anguloEmGraus * (double)Math.PI / 180.0F);
        }

        /// <summary>
        /// converte um ângulo em radianos para um ângulo em graus.
        /// </summary>
        /// <param name="anguloEmRadianos">ângulo em graus a ser convertido.</param>
        /// <returns>retorna um ângulo em graus.</returns>
        public static double toGraus(double anguloEmRadianos)
        {
            return (anguloEmRadianos * (180.0F / (double)Math.PI));
        }

        /// <summary>
        /// rotaciona um vetor 2D em um angulo determinado de incremento.
        /// </summary>
        /// <param name="anguloEmGraus">angulo em graus de incremento, utilizado na rotacao.</param>
        /// <param name="v">vetor 2D a ser rotacionado.</param>
        /// <returns>retorna um vetor2 rotacionado em um angulo de [anguloEmGraus].</returns>
        public static vetor2 rotacionaVetor(double anguloEmGraus, vetor2 v)
        {
            vetor2 vf = new vetor2(0.0F, 0.0F);
            double angle = (double)vetor3.toRadianos(anguloEmGraus);
            double cosAngle = (double)Math.Cos(angle);
            double sinAngle = (double)Math.Sin(angle);
            vf.X = v.X * cosAngle - v.Y * sinAngle;
            vf.Y = v.Y * cosAngle + v.X * sinAngle;
            return (vf);
        } // rotacionaVetor()

        /// <summary>
        /// rotaciona uma matriz [1,2] em [anguloEmGraus] graus.
        /// </summary>
        /// <param name="anguloEmGraus">ângulo em graus para a rotação.</param>
        /// <param name="mtv">matriz a ser rotacionada.</param>
        /// <returns>retorna uma matriz rotacionada pelo ângulo parâmetro.</returns>
        public static Matriz rotacionaVetor(double anguloEmGraus, Matriz mtv)
        {
            vetor2 vInicial = new vetor2((double)mtv.getElemento(0, 0), (double)mtv.getElemento(0, 1));
            vetor2 vFinal = rotacionaVetor(anguloEmGraus, vInicial);
            Matriz mtFinal = new Matriz(1, 2);
            mtFinal.setElemento(0, 0, (double)vFinal.X);
            mtFinal.setElemento(0, 1, (double)vFinal.Y);
            return (mtFinal);
        }
        /// <summary>
        /// muda a direção do vetor 2D para um determinado [anguloEmGraus].
        /// </summary>
        /// <param name="anguloEmGraus">novo ângulo-direção do vetor 2D de entrada.</param>
        /// <param name="v">estrutura representando o vetor 2D.</param>
        /// <returns></returns>
        public static vetor2 rotacionaVetorComAnguloAbsoluto(double anguloEmGraus, vetor2 v)
        {
            vetor2 vf = new vetor2(0.0, 0.0);
            double angle = vetor3.toRadianos(anguloEmGraus);
            double raio = Math.Sqrt(v.X * v.X + v.Y * v.Y);
            vf.X = raio * Math.Cos(angle);
            vf.Y = raio * Math.Sin(angle);

            return vf;

        } // rotacionaVetorComAnguloAbsoluto()

        /// <summary>
        /// rotaciona o vetor 3D em um determinado ângulo em seu plano XY, e detêrminado ângulo em seu eixo Z.
        /// Tais ângulos são absolutos. A rotação é feita em coordenadas esféricas.
        /// ATENÇÃO: O PLANO XY ORIGINAL FOI TROCADO PELO PLANO XZ, POIS A COORDENADA Z É A COORDENADA DE PROFUNDIDADE NOS DESENHOS.
        /// </summary>
        /// <param name="anguloXYEmGraus">ângulo absoluto para o plano XY, em Graus.</param>
        /// <param name="anguloZEmGraus">ângulo absoluto para o eixo Z.</param>
        /// <param name="v">vetor 3D a ser rotacionado.</param>
        /// <returns>retorna o vetor 3D de entrada com uma nova direção e sentido.</returns>
        public static vetor3 rotacionaVetorComAnguloAbsoluto(double anguloXYEmGraus, double anguloZEmGraus, vetor3 v)
        {
            vetor3 vf = new vetor3(0.0, 0.0, 0.0);
            double angleXY = vetor3.toRadianos((double)anguloXYEmGraus);
            double angleZ = vetor3.toRadianos((double)anguloZEmGraus);
            double raio = Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
            vf.X = raio * Math.Sin(angleZ) * Math.Cos(angleXY);
            vf.Z = raio * Math.Sin(angleZ) * Math.Sin(angleXY);
            vf.Y = raio * Math.Cos(angleZ);

            return vf;

        } // rotacionaVetorComAnguloAbsoluto()
        /// <summary>
        /// rotaciona o vetor com acréscimos de ângulos.A rotação é feita em coordenadas
        /// esféricas modificadas (eixo altura: Y, plano base: plano XZ).
        /// </summary>
        /// <param name="anguloXYEmGraus">acréscimo de ângulo no plano XY em Graus.</param>
        /// <param name="anguloZEmGraus">ácrescimo de ângulo no eixo Z.</param>
        /// <param name="v">vetor 3D a ser rotacionado.</param>
        /// <returns>retorna o vetor [v] rotacionado com acréscimos em graus.</returns>
        public static vetor3 rotacionaVetorComAnguloRelativo(double anguloXYEmGraus, double anguloZEmGraus, vetor3 v)
        {
            // calcula os ângulos iniciais, para somar aos ângulos parâmetros para um cálculo final de [rotacionaVetorComAnguloAbsoluto]
            double anguloZEmGrausInicial = v.encontraAnguloTeta();
            double anguloXYEmGRausInicial = v.encontraAnguloOmega();
            return rotacionaVetorComAnguloAbsoluto(anguloXYEmGraus + anguloXYEmGRausInicial, anguloZEmGraus + anguloZEmGrausInicial, v);
        } // rotacionaVetorComAnguloRelativo()
    } // class angulos
}// namespace MatrizLibrary
