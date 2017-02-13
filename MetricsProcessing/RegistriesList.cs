using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;

namespace MetricsProcessing
{
    public class RegistriesList : List<Registry>
    {
        public DateTime EndTime { get; }

        public RegistriesList(List<Registry> list, DateTime endTime) : base(list)
        {
            EndTime = endTime;
        }

        public TimeSpan Duration => EndTime - this[0].Time;
        public Registry First => this[0];
        public bool IsEmpty => this.Count == 0;
        public DateTime StartTime => this[0].Time;

        //public Registry this[int index]
        //{
        //    get { return _registries[index]; }

        //    set { _registries[index] = value; }
        //}

        //public int Count => _registries.Count;



        //public bool IsReadOnly => false;

        //public void Add(Registry item)
        //{
        //    _registries.Add(item);
        //}

        //public void Clear()
        //{
        //    _registries.Clear();
        //}

        //public bool Contains(Registry item)
        //{
        //    return _registries.Contains(item);
        //}

        //public void CopyTo(Registry[] array, int arrayIndex)
        //{
        //    _registries.CopyTo(array, arrayIndex);
        //}

        //public IEnumerator<Registry> GetEnumerator()
        //{
        //    return _registries.GetEnumerator();
        //}

        //public int IndexOf(Registry item)
        //{
        //    return _registries.IndexOf(item);
        //}

        //public void Insert(int index, Registry item)
        //{
        //    _registries.Insert(index, item);
        //}

        //public bool Remove(Registry item)
        //{
        //    return _registries.Remove(item);
        //}

        //public void RemoveAt(int index)
        //{
        //    _registries.RemoveAt(index);
        //}

        //public void RemoveRange(int index, int count)
        //{
        //    _registries.RemoveRange(index, count);
        //}
    }
}
