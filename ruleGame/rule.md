Scripts
│
├── Core // Chứa hệ thống nền của game
│ ├── GameManager.cs // Trung tâm điều khiển toàn bộ game
│ ├── GameState.cs // Quản lý trạng thái game (Menu, Playing, Battle...)
│ ├── SceneLoader.cs // Chuyển đổi giữa các Scene
│ ├── TimeManager.cs // Quản lý thời gian trong game (ngày, tháng, năm)
│ ├── SaveManager.cs // Lưu và tải dữ liệu người chơi
│ ├── DataManager.cs // Quản lý việc load dữ liệu game
│ ├── EventBus.cs // Hệ thống sự kiện giao tiếp giữa các module
│ ├── RandomManager.cs // Quản lý random có kiểm soát
│ └── AudioManager.cs // Quản lý âm thanh toàn game
│
│
├── Player // Quản lý nhân vật người chơi
│ ├── PlayerController.cs // Điều khiển player trong game
│ ├── PlayerData.cs // Dữ liệu lưu của nhân vật
│ ├── PlayerStats.cs // Các chỉ số cơ bản của player
│ ├── PlayerFactory.cs // Tạo nhân vật mới
│ ├── PlayerManager.cs // Quản lý player hiện tại
│ └── PlayerLevel.cs // Quản lý cấp độ nhân vật
│
│
├── Character // Hệ thống nhân vật
│ ├── Character.cs // Đối tượng nhân vật cơ bản
│ ├── CharacterData.cs // Data nhân vật
│ ├── CharacterStats.cs // Các chỉ số nhân vật
│ ├── TalentSystem.cs // Hệ thống thiên phú
│ ├── SpiritRoot.cs // Hệ thống linh căn
│ ├── AttributeSystem.cs // Hệ thống thuộc tính
│ └── GrowthSystem.cs // Tăng trưởng nhân vật
│
│
├── Actions // Hệ thống hành động chung của game
│ ├── ActionManager.cs // Điều phối các hành động
│ ├── GameAction.cs // Định nghĩa một hành động
│ ├── ActionType.cs // Danh sách loại hành động
│ ├── ActionResult.cs // Kết quả sau khi hoàn thành action
│ └── ActionQueue.cs // Hàng đợi các hành động
│
│
├── Cultivation // Hệ thống tu luyện
│ ├── CultivationManager.cs // Điều khiển quá trình tu luyện
│ ├── CultivationData.cs // Dữ liệu tu luyện
│ ├── RealmSystem.cs // Quản lý cảnh giới
│ ├── BreakthroughSystem.cs // Hệ thống đột phá cảnh giới
│ ├── MeditationSystem.cs // Hệ thống ngồi thiền luyện công
│ ├── SpiritEnergy.cs // Quản lý linh khí
│ ├── TechniqueSystem.cs // Quản lý công pháp
│ └── CultivationCalculator.cs // Tính toán tốc độ tu luyện
│
│
├── Combat // Hệ thống chiến đấu
│ ├── BattleManager.cs // Điều khiển trận đấu
│ ├── BattleUnit.cs // Đối tượng tham gia chiến đấu
│ ├── BattleState.cs // Trạng thái trận đấu
│ ├── DamageCalculator.cs // Tính sát thương
│ ├── SkillSystem.cs // Hệ thống kỹ năng
│ ├── BuffSystem.cs // Buff và debuff
│ ├── Enemy.cs // Đối tượng kẻ địch
│ └── AIController.cs // Điều khiển AI chiến đấu
│
│
├── Trading // Hệ thống buôn bán
│ ├── TradingManager.cs // Điều phối giao dịch
│ ├── Shop.cs // Cửa hàng
│ ├── MarketSystem.cs // Thị trường
│ ├── PriceSystem.cs // Tính giá vật phẩm
│ ├── BuySystem.cs // Xử lý mua hàng
│ ├── SellSystem.cs // Xử lý bán hàng
│ └── TradeTransaction.cs // Lịch sử giao dịch
│
│
├── Production // Hệ thống sản xuất
│ ├── ProductionManager.cs // Quản lý sản xuất
│ ├── RecipeSystem.cs // Công thức chế tạo
│ ├── CraftingSystem.cs // Chế tạo vật phẩm
│ ├── ProductionQueue.cs // Hàng đợi sản xuất
│ ├── ResourceCost.cs // Chi phí nguyên liệu
│ └── ProductionResult.cs // Kết quả sản xuất
│
│
├── Inventory // Hệ thống kho đồ
│ ├── InventoryManager.cs // Quản lý kho
│ ├── Item.cs // Định nghĩa vật phẩm
│ ├── ItemData.cs // Dữ liệu vật phẩm
│ ├── ItemStack.cs // Nhóm vật phẩm cùng loại
│ ├── Equipment.cs // Trang bị
│ └── EquipmentSystem.cs // Trang bị và nâng cấp
│
│
├── World // Thế giới trong game
│ ├── WorldManager.cs // Quản lý thế giới
│ ├── Location.cs // Địa điểm
│ ├── MapSystem.cs // Hệ thống bản đồ
│ ├── NPC.cs // Nhân vật NPC
│ ├── ResourceNode.cs // Điểm tài nguyên
│ └── WorldEvent.cs // Sự kiện thế giới
│
│
├── Quest // Nhiệm vụ
│ ├── QuestManager.cs // Quản lý nhiệm vụ
│ ├── QuestData.cs // Dữ liệu nhiệm vụ
│ ├── QuestCondition.cs // Điều kiện hoàn thành
│ └── QuestReward.cs // Phần thưởng nhiệm vụ
│
│
├── Data // Load và quản lý dữ liệu
│ ├── ExcelReader.cs // Đọc dữ liệu Excel
│ ├── JsonReader.cs // Đọc dữ liệu JSON
│ ├── ConfigManager.cs // Quản lý config game
│ ├── CharacterConfig.cs // Config nhân vật
│ ├── ItemConfig.cs // Config vật phẩm
│ ├── SkillConfig.cs // Config kỹ năng
│ └── Database.cs // Truy cập dữ liệu chung
│
│
├── UI // Giao diện người chơi
│ ├── UIManager.cs // Quản lý UI
│ ├── MainMenuUI.cs // Menu chính
│ ├── CharacterUI.cs // Màn hình nhân vật
│ ├── CultivationUI.cs // UI tu luyện
│ ├── BattleUI.cs // UI chiến đấu
│ ├── ShopUI.cs // UI cửa hàng
│ ├── InventoryUI.cs // UI kho đồ
│ └── PopupUI.cs // Popup thông báo
│
│
├── Utils // Các hàm dùng chung
│ ├── Extensions.cs // Extension method
│ ├── MathUtils.cs // Hàm toán học
│ ├── StringUtils.cs // Xử lý chuỗi
│ └── DateUtils.cs // Xử lý thời gian
│
└── Editor // Script chỉ chạy trong Unity Editor
├── PlayFromFirstScene.cs // Luôn chạy Scene đầu tiên khi Play
└── DataExporter.cs // Tool export dữ liệu trong Editor
