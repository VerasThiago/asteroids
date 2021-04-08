public class PicoEDA {
    public int id=0;
    public double value=0; //valor eda
    public double time=0; //tempo que foi coletado em Unixtimestamp
    public double size=0; //diferença no valor eda entre o pico atual e o pico anterior

    public PicoEDA(int id, double time, double value, double size) {
        this.id = id;
        this.time = time;
        this.value = value;
        this.size = size;
    }

    public PicoEDA(EDASignal eda, double size) {
        this.id = eda.id;
        this.time = eda.time;
        this.value = eda.value;
        this.size = size;
    }
}