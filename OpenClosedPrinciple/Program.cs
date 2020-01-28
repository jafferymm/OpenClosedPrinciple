using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static System.Console;

namespace OpenClosedPrinciple
{
    public class Program
    {
        public enum Color
        {
            Red, Blue, Green
        }

        public enum Size
        {
            Small, Medium, Large, Huge
        }

        public class Product
        {
            public string Name;
            public Color Color;
            public Size Size;

            public Product(string name, Color color, Size size)
            {
                if (name == null)
                {
                    throw new ArgumentNullException(paramName: nameof(name));
                }

                Name = name;
                Color = color;
                Size = size;
            }
        }

        //ideally this class should be OpenForExtension and yet ClosedForModification
        //OpenForExtension using inheritance, that is using Interfaces
        public class ProductFilter
        {
            //this method might have already been shipped to a customer
            public IEnumerable<Product> FilterBySize(IEnumerable<Product> products, Size size)
            {
                foreach (var p in products)
                {
                    if (p.Size == size)
                        yield return p;
                }
            }

            public IEnumerable<Product> FilterByColor(IEnumerable<Product> products, Color color)
            {
                foreach (var p in products)
                {
                    if (p.Color == color)
                        yield return p;
                }
            }

            public IEnumerable<Product> FilterBySizeAndColor(IEnumerable<Product> products, Size size, Color color)
            {
                foreach (var p in products)
                {
                    if (p.Color == color && p.Size == size)
                        yield return p;
                }
            }
        }


        public interface ISpecification<T>
        {
            bool IsSatified(T t);
        }

        public interface IFilter<T>
        {
            IEnumerable<T> Filter(IEnumerable<T> items, ISpecification<T> spec);
        }

        public class ColorSpecification : ISpecification<Product>
        {
            private Color color;
            public ColorSpecification(Color color)
            {
                this.color = color;
            }
            public bool IsSatified(Product t)
            {
                return t.Color == color;
            }
        }

        public class SizeSpecification : ISpecification<Product>
        {
            private Size size;

            public SizeSpecification(Size size)
            {
                this.size = size;
            }
            public bool IsSatified(Product t)
            {
                return t.Size == size;
            }
        }

        public class AndSpecification<T> : ISpecification<T>
        {

            private ISpecification<T> first, second;

            public AndSpecification(ISpecification<T> first, ISpecification<T> second)
            {
                if (first == null)
                {
                    throw new ArgumentNullException(paramName: nameof(first));
                }
                if (second == null)
                {
                    throw new ArgumentNullException(paramName: nameof(second));
                }
                this.first = first;
                this.second = second;
            }
            public bool IsSatified(T t)
            {
                return first.IsSatified(t) && second.IsSatified(t);
            }
        }

        public class BetterFilter : IFilter<Product>
        {
            public IEnumerable<Product> Filter(IEnumerable<Product> items, ISpecification<Product> spec)
            {
                foreach (var i in items)
                {
                    if (spec.IsSatified(i))
                        yield return i;
                }
            }
        }

        static void Main(string[] args)
        {
            var apple = new Product("Apple", Color.Green, Size.Small);
            var tree = new Product("Tree", Color.Green, Size.Large);
            var house = new Product("House", Color.Blue, Size.Large);

            Product[] products = { apple, tree, house };

            //b4 priciple 
            var pf = new ProductFilter();
            Console.WriteLine("Green Products (old):");

            foreach (var p in pf.FilterByColor(products, Color.Green))
            {
                Console.WriteLine($" - {p.Name}");
            }

            //after applying principles
            var bf = new BetterFilter();
            Console.WriteLine("Green Products (new):");

            foreach (var p in bf.Filter(products, new ColorSpecification(Color.Green)))
            {
                Console.WriteLine($" - {p.Name}");
            }

            Console.WriteLine("Large Products (new):");
            foreach (var p in bf.Filter(products, new SizeSpecification(Size.Large)))
            {
                Console.WriteLine($" - {p.Name}");
            }

            Console.WriteLine("Large Blue Products (new):");
            foreach (var p in bf.Filter(products, new AndSpecification<Product>(new ColorSpecification(Color.Blue), new SizeSpecification(Size.Large))))
            {
                Console.WriteLine($" - {p.Name} is large and blue");
            }


            Console.ReadLine();
        }
    }
}

