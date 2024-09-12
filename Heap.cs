using System.ComponentModel.Design.Serialization;

namespace PathFinding
{
    public class MinHeap<T> where T:IHeapItem<T>{
        public List<T> Data {get;protected set;}
        public int Count {get; protected set;}

        public MinHeap(){
            this.Data = new List<T>();
            this.Count = 0;
        }


        public MinHeap(List<T> unsortedData):base(){
            this.Data =  new List<T>(unsortedData.Count);
            foreach(var i in unsortedData){
                this.Insert(i);
            }
        }

        public void Insert(T num){
            num.HeapIndex = this.Count;
            this.Data.Add(num);
            this.Count++;
            this.HeapUp();
        }

        public T Peek(){
            return this.Data[0];
        }

        public T Pop(){
            T result = this.Peek();
            
            this.Data[0] = this.Data.Last();    // put last element at root
            this.Data[0].HeapIndex = 0;
            this.Data.RemoveAt(Data.Count-1);   // removed last element
            
            this.HeapDown();
            this.Count--;
            return result;

        }

        virtual protected void HeapUp(){
            int index = this.Data.Count-1;
            HeapUp(index);
        }
        
        virtual protected void HeapDown(){
            int index = 0;
            while(index*2+1<this.Data.Count){
                if(index*2+2 > this.Data.Count-1){
                    if(this.Data[index].CompareTo(this.Data[index*2+1]) > 0){
                        (this.Data[index].HeapIndex, this.Data[index*2+1].HeapIndex) = (this.Data[index*2+1].HeapIndex, this.Data[index].HeapIndex);
                        (this.Data[index], this.Data[index*2+1]) = (this.Data[index*2+1], this.Data[index]);
                    }
                    break;
                }
                else{
                    int minIndex = this.Data[index*2+1].CompareTo(this.Data[index*2+2]) < 0? index*2+1 : index*2+2; 
                    if(this.Data[index].CompareTo(this.Data[minIndex]) > 0){
                        (this.Data[index].HeapIndex , this.Data[minIndex].HeapIndex) = (this.Data[minIndex].HeapIndex,this.Data[index].HeapIndex);
                        (this.Data[index],this.Data[minIndex]) = (this.Data[minIndex],this.Data[index]);
                        index = minIndex;
                    }
                    else{
                        break;
                    }
                }
            }
        }

        protected void HeapUp(int index){
            while(this.Data[index].CompareTo(this.Data[(index-1)/2])<0 && index > 0){
                (this.Data[index].HeapIndex , this.Data[(index-1)/2].HeapIndex) = (this.Data[(index-1)/2].HeapIndex , this.Data[index].HeapIndex);
                (this.Data[index], this.Data[(index-1)/2]) = (this.Data[(index-1)/2], this.Data[index]); // swap
                index = (index-1)/2;
            }
        }
        public void UpdateHeap(T item){
            int index = item.HeapIndex;
            HeapUp(index);
        }
    }

    public class MaxHeap<T>:MinHeap<T> where T:IHeapItem<T>{
        public MaxHeap():base(){
            ;
        }
        public MaxHeap(List<T> unsortedData) : base(unsortedData){
            ;
        }

        protected override void HeapDown()
        {
            int index = 0;
            while(index*2+1<this.Data.Count){
                if(index*2+2 > this.Data.Count-1){
                    if(this.Data[index].CompareTo(this.Data[index*2+1]) < 0){
                        (this.Data[index].HeapIndex , this.Data[index*2+1].HeapIndex) = (this.Data[index*2+1].HeapIndex , this.Data[index].HeapIndex);
                        (this.Data[index], this.Data[index*2+1]) = (this.Data[index*2+1], this.Data[index]);
                    }
                    break;
                }
                else{
                    int minIndex = this.Data[index*2+1].CompareTo(this.Data[index*2+2]) > 0 ? index*2+1 : index*2+2; 
                    if(this.Data[index].CompareTo(this.Data[minIndex])<0){
                        (this.Data[index].HeapIndex,this.Data[minIndex].HeapIndex) = (this.Data[minIndex].HeapIndex,this.Data[index].HeapIndex);
                        (this.Data[index],this.Data[minIndex]) = (this.Data[minIndex],this.Data[index]);
                        index = minIndex;
                    }
                    else{
                        break;
                    }
                }
            }
        }

        protected override void HeapUp()
        {
            int index = this.Data.Count-1;
            while(this.Data[index].CompareTo(this.Data[(index-1)/2]) > 0 && index > 0){
                (this.Data[index].HeapIndex , this.Data[(index-1)/2].HeapIndex) = (this.Data[(index-1)/2].HeapIndex , this.Data[index].HeapIndex);
                (this.Data[index], Data[(index-1)/2]) = (this.Data[(index-1)/2], this.Data[index]); // swap
                index = (index-1)/2;
            }
        }

    }

    public interface IHeapItem<T>: IComparable<T>{
        int HeapIndex{
            get;
            set;
        }
    }
}