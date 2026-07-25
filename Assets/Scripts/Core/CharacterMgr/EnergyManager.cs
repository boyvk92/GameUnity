

public class EnergyManager{
    private int energyWeek = 0;
    private int energyWeekMax = 0;
    private int energyWeekNext = 0;

    public EnergyManager(int? _energyWeek, int? _energyWeekMax, int? _energyWeekNext){
        this.energyWeek = _energyWeek ?? 100;
        this.energyWeekMax = _energyWeekMax ?? 100;
        this.energyWeekNext = _energyWeekNext ?? 100;
    }


    public int getEnergyWeek(){
        return this.energyWeek;
    } 

    public int getEnergyWeekMax(){
        return this.energyWeekMax;
    }

    public void nextWeek(){
        this.energyWeekMax = this.energyWeekNext;
        this.energyWeek = this.energyWeekMax;
    }

    public bool useEnergyWeek(int energy){
        if(energy > this.energyWeek){
            return false; 
        }

        this.energyWeek -= energy;
        return true;
    }

    public void energyNextUp(int energy){
        this.energyWeekNext += energy;
    }
}