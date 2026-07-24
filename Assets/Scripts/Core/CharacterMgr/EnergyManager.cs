

public class EnergyManager{
    private int energyWeed = 0;
    private int energyWeedMax = 0;
    private int energyWeedNext = 0;

    public EnergyManager(int _energyWeed, int _energyWeedMax, int _energyWeedNext){
        this.energyWeed = _energyWeed;
        this.energyWeedMax = _energyWeedMax;
        this.energyWeedNext = _energyWeedNext;
    }


    public int getEnergyWeed(){
        return this.energyWeed;
    } 

    public int getEnergyWeedMax(){
        return this.energyWeedMax;
    }

    public void nextWeek(){
        this.energyWeedMax = this.energyWeedNext;
        this.energyWeed = this.energyWeedMax;
    }

    public bool useEnergy(int energy){
        if(energy > this.energyWeed){
            return false; 
        }

        this.energyWeed -= energy;
        return true;
    }

    public void energyNextUp(int energy){
        this.energyWeedNext += energy;
    }
}