using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace AlgorytmyGrafowe {
    class Krawedz : IComparable {
        public int poczatek { get; }
        public int koniec {get; }
        public Krawedz(int p, int k) {
            poczatek = p;
            koniec = k;
        }
        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            return (poczatek == (obj as Krawedz).poczatek) && (koniec == (obj as Krawedz).koniec);
        }
        public override int GetHashCode() {
            return (poczatek, koniec).GetHashCode();
        }
        public static bool operator ==(Krawedz a, Krawedz b) {
            if (a is null) {
                if (b is null) {
                    return true;
                }
                return false;
            }
            return a.Equals(b);
        }
        public static bool operator !=(Krawedz a, Krawedz b) {
            return !(a == b);
        }
        public int CompareTo(object obj) {
            if (obj == null) {
                return 1;
            }
            Krawedz k = obj as Krawedz;
            if (k != null) {
                if (poczatek < k.poczatek) {
                    return -1;
                }
                if (poczatek > k.poczatek) {
                    return 1;
                }
                return koniec.CompareTo(k.koniec);
            }
            throw new ArgumentException("Objekt nie jest krawedzia");
        }
    }
    abstract class Graf {
        protected int rzad;
        public int Rzad() {
            return rzad;
        }
        public abstract void Wypisz();
        public abstract bool DodajKrawedz(Krawedz krawedz);
        public abstract bool DodajKrawedz(int poczatek, int koniec);
        public abstract bool CzyKrawedz(Krawedz krawedz);
        public abstract bool CzyKrawedz(int poczatek, int koniec);
        public abstract List<int> Nastepniki(int wierzcholek);
        public abstract List<int> Poprzedniki(int wierzcholek);
        public List<List<int>> AlgorytmDFS() {
            List<List<int>> wynik = new List<List<int>>();
            bool[] odwiedzone = new bool[rzad];
            for (int i = 0; i < rzad; i++) {
                if (!odwiedzone[i]) {
                    List<int> fragment = new List<int>();
                    odwiedzone[i] = true;
                    fragment.Add(i);
                    KrokDFS(i, fragment, odwiedzone);
                    wynik.Add(fragment);
                }
            }
            return wynik;
        }
        protected void KrokDFS(int wierzcholek, List<int> wynik, bool[] odwiedzone) {
            List<int> nastepniki = Nastepniki(wierzcholek);
            foreach (int nastepnik in nastepniki) {
                if (!odwiedzone[nastepnik]) {
                    odwiedzone[nastepnik] = true;
                    wynik.Add(nastepnik);
                    KrokDFS(nastepnik, wynik, odwiedzone);
                }
            }
        }
        public List<int> AlgorytmKahna() {
            if (rzad <= 0) {
                return null;
            }
            SortedList<int, List<int>> poprzedniki = new SortedList<int, List<int>>();
            List<int> wynik = new List<int>();
            for (int i = 0; i < rzad; i++) {
                poprzedniki.Add(i, Poprzedniki(i));
            }
            while(poprzedniki.Count > 0) {
                int wierzcholekNiezalezny = -1;
                foreach (KeyValuePair<int, List<int>> p in poprzedniki) {
                    if (p.Value.Count == 0) {
                        wierzcholekNiezalezny = p.Key;
                        poprzedniki.Remove(wierzcholekNiezalezny);
                        break;
                    }
                }
                if (wierzcholekNiezalezny == -1) {
                    return null;
                }
                wynik.Add(wierzcholekNiezalezny);
                foreach (int nastepnik in Nastepniki(wierzcholekNiezalezny)){
                    poprzedniki[nastepnik].Remove(wierzcholekNiezalezny);
                }
            }
            return wynik;
        }
        
    }
    class MacierzSasiedztwa : Graf {
        protected int[,] macierz;
        public MacierzSasiedztwa(int rzadGrafu) {
            if (rzadGrafu < 1) {
                rzad = 0;
                macierz = null;
                return;
            }
            rzad = rzadGrafu;
            macierz = new int[rzad, rzad];
        }
        public override void Wypisz() {
            for (int i = 0; i < rzad; i++) {
                for (int j = 0; j < rzad; j++) {
                    Console.Write(String.Format("{0,3}", macierz[i, j].ToString()));
                }
                Console.Write("\n");
            }
        }
        public override bool DodajKrawedz(Krawedz k) {
            return DodajKrawedz(k.poczatek, k.koniec);
        }
        public override bool DodajKrawedz(int poczatek, int koniec) {
            if (poczatek < 0 || poczatek >= rzad || koniec < 0 || koniec >= rzad) {
                return false;
            }
            if (macierz[poczatek, koniec] == 1 || macierz[koniec, poczatek] == 1) {
                return false;
            }
            macierz[poczatek, koniec] = 1;
            macierz[koniec, poczatek] = -1;
            return true;
        }
        public override bool CzyKrawedz(Krawedz k) {
            return CzyKrawedz(k.poczatek, k.koniec);
        }
        public override bool CzyKrawedz(int poczatek, int koniec) {
            if (poczatek < 0 || poczatek >= rzad || koniec < 0 || koniec >= rzad) {
                return false;
            }
            return (macierz[poczatek, koniec] == 1);
        }
        public override List<int> Nastepniki(int wierzcholek) {
            if (wierzcholek < 0 || wierzcholek >= rzad) {
                return null;
            }
            List<int> nastepniki = new List<int>();
            for (int i = 0; i < rzad; i++) {
                if (macierz[wierzcholek, i] == 1) {
                    nastepniki.Add(i);
                }
            }
            return nastepniki;
        }
        public override List<int> Poprzedniki(int wierzcholek) {
            if (wierzcholek< 0 || wierzcholek >= rzad) {
                return null;
            }
            List<int> poprzedniki = new List<int>();
            for (int i = 0; i < rzad; i++) {
                if (macierz[i, wierzcholek] == 1) {
                    poprzedniki.Add(i);
                }
            }
            return poprzedniki;
        }
    }
    class ListaKrawedzi : Graf {
        protected List<Krawedz> krawedzie;
        public ListaKrawedzi(int rzadGrafu) {
            if (rzadGrafu < 1) {
                rzad = 0;
                krawedzie = null;
                return;
            }
            krawedzie = new List<Krawedz>();
            rzad = rzadGrafu;
        }
        public override void Wypisz() {
            if (krawedzie == null) {
                return;
            }
            for (int i = 0; i < krawedzie.Count; i++) {
                Console.WriteLine(String.Format("{0,5}", krawedzie[i].poczatek.ToString()) + String.Format("{0,5}", krawedzie[i].koniec.ToString()));
            }
        }
        public override bool DodajKrawedz(Krawedz k) {
            if (k.poczatek < 0 || k.poczatek >= rzad || k.koniec < 0 || k.koniec >= rzad){
                return false;
            }
            int index = krawedzie.BinarySearch(k);
            if (index < 0) {
                krawedzie.Insert(~index, k);
                return true;
            }
            return false;
        }
        public override bool DodajKrawedz(int poczatek, int koniec) {
            return DodajKrawedz(new Krawedz(poczatek, koniec));
        }
        public override bool CzyKrawedz(Krawedz k) {
            return krawedzie.BinarySearch(k) >= 0 ? true : false;
        }
        public override bool CzyKrawedz(int poczatek, int koniec) {
            return CzyKrawedz(new Krawedz(poczatek, koniec));
        }
        public override List<int> Nastepniki(int wierzcholek) {
            if (wierzcholek < 0 || wierzcholek >= rzad) {
                return null;
            }
            List<int> nastepniki = new List<int>();
            foreach (Krawedz krawedz in krawedzie) {
                if (krawedz.poczatek == wierzcholek) {
                    nastepniki.Add(krawedz.koniec);
                }
            }
            return nastepniki;
        }
        public override List<int> Poprzedniki(int wierzcholek) {
            if (wierzcholek < 0 || wierzcholek >= rzad) {
                return null;
            }
            List<int> poprzedniki = new List<int>();
            foreach (Krawedz krawedz in krawedzie) {
                if (krawedz.koniec == wierzcholek) {
                    int index = poprzedniki.BinarySearch(krawedz.koniec);
                    if (index < 0) {
                        poprzedniki.Insert(~index, krawedz.poczatek);
                    }
                }
            }
            return poprzedniki;
        }
    }
    class ListaNastepnikow : Graf {
        protected List<int>[] nastepniki;
        public ListaNastepnikow(int rzadGrafu) {
            if (rzadGrafu < 1) {
                rzad = 0;
                nastepniki = null;
                return;
            }
            rzad = rzadGrafu;
            nastepniki = new List<int>[rzad];
            for (int i = 0; i < rzad; i++) {
                nastepniki[i] = new List<int>();
            }
        }
        public override void Wypisz() {
            if (nastepniki == null) {
                return;
            }
            for (int i = 0; i < nastepniki.Length; i++) {
                Console.Write(String.Format("{0,5}", i.ToString()) + ":");
                for (int j = 0; j < nastepniki[i].Count; j++) {
                    Console.Write(String.Format("{0,5}", nastepniki[i][j].ToString()));
                }
                Console.Write("\n");
            }
        }
        public override bool DodajKrawedz(Krawedz k) {
            return DodajKrawedz(k.poczatek, k.koniec);
        }
        public override bool DodajKrawedz(int poczatek, int koniec) {
            if (poczatek < 0 || poczatek >= rzad || koniec < 0 || koniec >= rzad) {
                return false;
            }
            int index = nastepniki[poczatek].BinarySearch(koniec);
            if (index < 0) {
                nastepniki[poczatek].Insert(~index, koniec);
                return true;
            }
            return false;
        }
        public override bool CzyKrawedz(Krawedz k) {
            return CzyKrawedz(k.poczatek, k.koniec);
        }
        public override bool CzyKrawedz(int poczatek, int koniec) {
            if (poczatek < 0 || poczatek >= rzad || koniec < 0 || koniec >= rzad) {
                return false;
            }
            return nastepniki[poczatek].Contains(koniec);
        }
        public override List<int> Nastepniki(int wierzcholek) {
            if (wierzcholek < 0 || wierzcholek >= rzad){
                return null;
            }
            return nastepniki[wierzcholek];
        }
        public override List<int> Poprzedniki(int wierzcholek) {
            if (wierzcholek < 0 || wierzcholek >= rzad){
                return null;
            }
            List<int> poprzedniki = new List<int>();
            for (int i = 0; i < nastepniki.Length; i++) {
                if (nastepniki[i].BinarySearch(wierzcholek) >= 0) {
                    poprzedniki.Add(i);
                }
            }
            return poprzedniki;
        }
    }
    class Program {
        static void Main(string[] args) {
            Random random = new Random();
            for (int n = 100; n <= 500; n += 100) {
                Console.WriteLine("n = " + n.ToString());
                for (int t = 0; t < 10; t++) {
                    Stopwatch stopwatch = new Stopwatch();
                    Graf macierzSasiedztwa = new MacierzSasiedztwa(n);
                    Graf listaKrawedzi = new ListaKrawedzi(n);
                    Graf listaNastepnikow = new ListaNastepnikow(n);
                    List<Krawedz> krawedzie = new List<Krawedz>();
                    for (int i = 0; i < n*(n-1)/4; i++) {
                        int a = random.Next(1, n - 1);
                        int b = random.Next(a, n);
                        krawedzie.Add(new Krawedz(a, b));
                    }

                    // Tworzenie struktury
                    // Macierz sasiedztwa
                    stopwatch = Stopwatch.StartNew();
                    foreach (Krawedz k in krawedzie) {
                        macierzSasiedztwa.DodajKrawedz(k);
                    }
                    stopwatch.Stop();
                    Console.Write(stopwatch.ElapsedMilliseconds.ToString() + " ");
                    // Lista krawedzi
                    stopwatch = Stopwatch.StartNew();
                    foreach (Krawedz k in krawedzie) {
                        listaKrawedzi.DodajKrawedz(k);
                    }
                    stopwatch.Stop();
                    Console.Write(stopwatch.ElapsedMilliseconds.ToString() + " ");
                    // Lista nastepnikow
                    stopwatch = Stopwatch.StartNew();
                    foreach (Krawedz k in krawedzie) {
                        listaNastepnikow.DodajKrawedz(k);
                    }
                    stopwatch.Stop();
                    Console.Write(stopwatch.ElapsedMilliseconds.ToString() + " : ");

                    // Algorytm Kahn'a
                    // Macierz sasiedztwa
                    stopwatch = Stopwatch.StartNew();
                    macierzSasiedztwa.AlgorytmKahna();
                    stopwatch.Stop();
                    Console.Write(stopwatch.ElapsedMilliseconds.ToString() + " ");
                    // Lista krawedzi
                    stopwatch = Stopwatch.StartNew();
                    listaKrawedzi.AlgorytmKahna();
                    stopwatch.Stop();
                    Console.Write(stopwatch.ElapsedMilliseconds.ToString() + " ");
                    // Lista nastepnikow
                    stopwatch = Stopwatch.StartNew();
                    listaNastepnikow.AlgorytmKahna();
                    stopwatch.Stop();
                    Console.Write(stopwatch.ElapsedMilliseconds.ToString() + " : ");

                    // Algorytm DFS
                    // Macierz sasiedztwa
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < n; i++) {
                        macierzSasiedztwa.AlgorytmDFS();
                    }
                    stopwatch.Stop();
                    Console.Write(stopwatch.ElapsedMilliseconds.ToString() + " ");
                    // Lista krawedzi
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < n; i++) {
                        listaKrawedzi.AlgorytmDFS();
                    }
                    stopwatch.Stop();
                    Console.Write(stopwatch.ElapsedMilliseconds.ToString() + " ");
                    // Lista nastepnikow
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < n; i++) {
                        listaNastepnikow.AlgorytmDFS();
                    }
                    stopwatch.Stop();
                    Console.Write(stopwatch.ElapsedMilliseconds.ToString() + "\n");
                }
            }
        }
    }
}
