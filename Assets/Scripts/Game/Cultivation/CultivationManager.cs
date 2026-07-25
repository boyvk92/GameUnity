public class CultivationManager{

    public CultivationType CultivationCalculator(){
        // tính toán
       
        float linhCan = 1;

        Cultivation cultivation = GameManager.Instance.getCharactorStats().Cultivation;
       
        float cultivationSpeed = cultivation.CultivationSpeed;
        //cong phap
        float congphap = 0.2f; // tính theo chu kỳ
        int timeUse = 80; // t timeUse

        //moi trường 
        float linhKhi = 1;

        return new CultivationType(timeUse, linhKhi * linhCan * cultivationSpeed * congphap);
    }

    public TrainType TrainCalculator(){
        // tính toán
        TrainType trainType = new TrainType();

        trainType.timeUse = 80;
        trainType.energyUse = 10;

        trainType.attribute =  new CharactorAttributesType();
        trainType.attribute.Strength = 10;


        return trainType;
    }
}
