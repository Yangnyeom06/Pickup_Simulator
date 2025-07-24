using UnityEngine;


public enum TimeOfDay
{
    Day,
    Evening
    
}
[System.Serializable]
public class GameDate
{
    public float timerTime;
    public int year;
    public int month;
    public int day;
    public TimeOfDay timeOfDay;
    public int pickUpItemCounts;
    public int sellItemCounts;
    public int todayGetMoney;
    public int todaySpendMoney;



    public GameDate(float timerTime, int year = 2013, int month = 3, int day = 7, TimeOfDay timeOfDay = TimeOfDay.Day, int pickUpItemCounts = 0, int sellItemCounts = 0, int todayGetMoney = 0, int todaySpendMoney = 0)
    {
        this.timerTime = timerTime;
        this.year = year;
        this.month = month;
        this.day = day;
        this.timeOfDay = timeOfDay;
        this.pickUpItemCounts = pickUpItemCounts;
        this.sellItemCounts = sellItemCounts;
        this.todayGetMoney = todayGetMoney;
        this.todaySpendMoney = todaySpendMoney;
    }

    public static GameDate FromData(asdfManager game)
    {
        return new GameDate(
            game.timerTime,
            game.year,
            game.month,
            game.day,
            game.timeOfDay,
            game.pickUpItemCounts,
            game.sellItemCounts,
            game.todayGetMoney,
            game.todaySpendMoney
        );
    }

    public void ApplyToGame(asdfManager game)
    {
        game.timerTime = timerTime;
        game.year = year;
        game.month = month;
        game.day = day;
        game.timeOfDay = timeOfDay;
        game.pickUpItemCounts = pickUpItemCounts;
        game.sellItemCounts = sellItemCounts;
        game.todayGetMoney = todayGetMoney;
        game.todaySpendMoney = todaySpendMoney;
        game.today.text = $"{month}/{day}";
    }

/*
                            public override string ToString()
                            {
                                string timeStr = timeOfDay == TimeOfDay.Day ? "낮" : "저녁";
                                return $"{year}년 {month}월 {day}일 ({timeStr})";
                            }
                            */
}