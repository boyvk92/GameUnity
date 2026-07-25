// Quản lý thời gian trong game (ngày, tháng, năm)

public class TypeTimeGame{
    public int week;
    public int month;
    public int year;


    public TypeTimeGame(int? _week, int? _month, int? _year){
        this.week = _week ?? 100;
        this.month = _month ?? 100;
        this.year = _year ?? 100;
    }
}

public class TimeManager{
 
    private TypeTimeGame timeGame;
    private int timeWeek;

    public TimeManager(){
        this.timeGame = new TypeTimeGame(1, 1, 1);
        this.timeWeek = this.getTimeFullWeek();
    }
    
    public int getTimeFullWeek(){
        return  25200; // 7*24*60
    }

    public TypeTimeGame nextWeek(){
        this.timeGame.week++;

        if(this.timeGame.week > 4){
            this.timeGame.week = 1;
            this.timeGame.month++;

            if(this.timeGame.month > 12){
                this.timeGame.month = 1;
                this.timeGame.year++;
            }
        }

        this.timeWeek = this.getTimeFullWeek();

        return this.timeGame;
    }

    public TypeTimeGame getTimeGame(){
        return this.timeGame;
    }

    public int getTimeWeek(){
        return this.timeWeek;
    }

    public bool useTimeWeek(int timeUse){
        if(timeUse > this.timeWeek){
            return false; 
        }

        this.timeWeek -= timeUse;
        return true;
    }

}