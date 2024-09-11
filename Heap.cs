using System.ComponentModel.Design.Serialization;

namespace PathFinding
{
    public class MinHeap{
        public List<int> Data {get;protected set;}
        public int Count {get; protected set;}

        public MinHeap(){
            this.Data = new List<int>();
            this.Count = 0;
        }


        public MinHeap(List<int> unsortedData):base(){
            this.Data =  new List<int>(unsortedData.Count);
            foreach(int i in unsortedData){
                this.Insert(i);
            }
        }

        public void Insert(int num){
            this.Data.Add(num);
            this.Count++;
            this.HeapUp();
        }

        public int Peek(){
            return this.Data[0];
        }

        public int Pop(){
            int result = this.Peek();
            this.Data[0] = this.Data.Last();    // put last element at root
            this.Data.RemoveAt(Data.Count-1);   // removed last element
            this.HeapDown();
            this.Count--;
            return result;

        }

        virtual protected void HeapUp(){
            int index = this.Data.Count-1;
            while(this.Data[index] < this.Data[(index-1)/2] && index > 0){
                (this.Data[index], Data[(index-1)/2]) = (this.Data[(index-1)/2], this.Data[index]); // swap
                index = (index-1)/2;
            }
        }
        
        virtual protected void HeapDown(){
            int index = 0;
            while(index*2+1<this.Data.Count){
                if(index*2+2 > this.Data.Count-1){
                    if(this.Data[index]>this.Data[index*2+1]){
                        (this.Data[index], this.Data[index*2+1]) = (this.Data[index*2+1], this.Data[index]);
                    }
                    break;
                }
                else{
                    int minIndex = this.Data[index*2+1] < this.Data[index*2+2] ? index*2+1 : index*2+2; 
                    if(this.Data[index]>this.Data[minIndex]){
                        (this.Data[index],this.Data[minIndex]) = (this.Data[minIndex],this.Data[index]);
                        index = minIndex;
                    }
                    else{
                        break;
                    }
                }
            }
        }
    }

    public class MaxHeap:MinHeap{
        public MaxHeap():base(){
            ;
        }
        public MaxHeap(List<int> unsortedData) : base(unsortedData){
            ;
        }

        protected override void HeapDown()
        {
            int index = 0;
            while(index*2+1<this.Data.Count){
                if(index*2+2 > this.Data.Count-1){
                    if(this.Data[index]<this.Data[index*2+1]){
                        (this.Data[index], this.Data[index*2+1]) = (this.Data[index*2+1], this.Data[index]);
                    }
                    break;
                }
                else{
                    int minIndex = this.Data[index*2+1] > this.Data[index*2+2] ? index*2+1 : index*2+2; 
                    if(this.Data[index]<this.Data[minIndex]){
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
            while(this.Data[index] > this.Data[(index-1)/2] && index > 0){
                (this.Data[index], Data[(index-1)/2]) = (this.Data[(index-1)/2], this.Data[index]); // swap
                index = (index-1)/2;
            }
        }

    }
}