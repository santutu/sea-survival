# HUD 코드 구조

## 🤔 왜 이런 구조로 설계했나?

### 문제 상황
실시간 게임에서 **UI 업데이트**를 어떻게 효율적으로 처리할까?

### 해결책: Event-Driven 구조
- **이벤트 기반** UI 업데이트
- **Observer 패턴**으로 느슨한 결합
- 성능과 유지보수성 확보

## 📊 HUD 코드 구조도

```
┌─────────────────┐    Event     ┌─────────────────┐
│   게임 로직     │ ─────────── ► │   UI 시스템     │
│                 │              │                 │
│ PlayerLevelSys  │ onExpGained  │ PlayerExpUI     │
│ Player.cs       │ onLevelUp    │ HealthBarUI     │
│ StageManager    │ onStageChange│ StageUI         │
└─────────────────┘              └─────────────────┘
         │                               │
         ▼                               ▼
┌─────────────────┐              ┌─────────────────┐
│   데이터 변경   │              │   UI 자동 반영  │
│                 │              │                 │
│ • 경험치 증가   │              │ • 경험치 바     │
│ • 레벨 증가     │              │ • 레벨 표시     │
│ • 체력 변화     │              │ • 체력 바       │
└─────────────────┘              └─────────────────┘
```

## 💻 핵심 코드 (경험치 시스템)

```csharp
// 경험치 획득 시 이벤트 발생 (PlayerLevelSystem.cs)
public void AddExp(int expAmount)
{
    currentExp += expAmount;
    onExpGained.Invoke(currentExp, GetExpRequiredForNextLevel());
    
    if (currentExp >= GetExpRequiredForNextLevel())
    {
        LevelUp();
    }
}

// UI에서 이벤트 수신 (PlayerExpUI.cs)
private void Start()
{
    PlayerLevelSystem.Ins.onExpGained.AddListener(UpdateExpBar);
    PlayerLevelSystem.Ins.onLevelUp.AddListener(UpdateLevelText);
}
```

### 🎯 결과
**데이터와 UI 분리**로 **확장성**과 **유지보수성** 확보

> 🎮 **GIF**: 경험치 획득 → UI 자동 업데이트 → 레벨업 이벤트 연쇄 반응 