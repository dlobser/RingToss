using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;


namespace MM.Msg {
	public interface IMessage {
		public string ToJson();
		public string GetUrl();
		public string PostUrl();

		public string PutUrl();
		public bool encrypted();
	}

	public class BaseRequest : IMessage {
		public virtual string ToJson() => JsonConvert.SerializeObject(this);

		public virtual string GetUrl() => string.Empty;

		public virtual string PostUrl() => string.Empty;
		public virtual string PutUrl() => string.Empty;


		public virtual bool encrypted() => false;
	}

	public class BaseResponse : IMessage {
		public virtual string ToJson() => JsonConvert.SerializeObject(this);

		public virtual string GetUrl() => string.Empty;

		public virtual string PostUrl() => string.Empty;

		public virtual string PutUrl() => string.Empty;

		public virtual bool encrypted() => false;
	}

	public class OperationResponse : BaseResponse {
		public OperationResult operation_result;
	}


	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);


	public class AuthResponse : OperationResponse {
		public AuthUser auth_user { get; set; }


		public class AuthUser {
			public User user { get; set; }
			public string token { get; set; }
		}
	}

	public class User {
		public string phone { get; set; }
		public string name { get; set; }
		public string surname { get; set; }
		public string email { get; set; }

		[CanBeNull] public string image { get; set; }
	}

	public class LeaderBoardUser {
		public int id { get; set; }
		public string name { get; set; }
		public string surname { get; set; }
		public int score { get; set; }
		public int rank { get; set; }
		public string image { get; set; }
	}

	public enum LeaderBoardDate {
		Day = 0,
		Week = 1,
		Month = 2,
		Year = 3
	}

	public class PrizeData {
		public int id { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		public string logo { get; set; }
		public string banner { get; set; }
		public int order { get; set; }
		public string type { get; set; }
	}
	public class WheelPrize
	{
		public string id { get; set; }
		public string logo { get; set; }
		public string name { get; set; }
		[CanBeNull] public string gold { get; set; }
		[CanBeNull] public string offerId { get; set; }
		// public double winProbability { get; set; }
		public bool isActive { get; set; }
	}


	public class UserImages {
		public int user_id;
		public string image;
	}

	public class Balance {
		public int gold_coins { get; set; }
		public int silver_coins { get; set; }
	}

	public class Category {
		public int id { get; set; }
		public string name { get; set; }
	}

	[Serializable]
	public class Vendor {
		public int id { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		[CanBeNull] public string image { get; set; }
	}

	public class VendorData {
		public Vendor vendor;
		public List<Offer> offers;
	}

	public class Game {
		public int id { get; set; }
		public string name { get; set; }
		public string image { get; set; }
		public string description { get; set; }
		public Balance balance { get; set; }
		public float coefficient { get; set; }
		public string applicationVersion { get; set; }
	}

	public class Transaction {
		public int type { get; set; }
		public DateTime createDt { get; set; }
		public int golds { get; set; }
		// public int silvers { get; set; }
		// public int endingBalance { get; set; }
		public string description { get; set; }
		public int gameId { get; set; }
		public int vendorId { get; set; }
		public int ordering { get; set; }
	}

	public class Offer {
		public int id;

		[CanBeNull] public string image { get; set; }
		public string offer_name { get; set; }
		public double price { get; set; }
		public int quantity { get; set; }

		public int quantity_per_user { get; set; }
		public int available_quantity { get; set; }

		public string start_date { get; set; }
		public string end_date { get; set; }
		public TimeSpan days_left { get; set; }

		public string short_description { get; set; }
		public string description { get; set; }
		public Category[] categories { get; set; }

		public bool coming_soon { get; set; }
		// public int buy_period { get; set; }
	}

	public class OfferData {
		public Offer offer;
		public Vendor vendor;
	}

	public class TopOffer {
		public int id { get; set; }
		public int offer_id { get; set; }
	}

	public class OwnedOfferData {
		public string code { get; set; }
		public bool isCodeActivated { get; set; }
		public string codeActivationDate { get; set; }
		public TimeSpan daysUntilCodeValidity { get; set; }
		public Offer offer { get; set; }
		public Vendor vendor { get; set; }
	}

	public class Offset {
		public int skip;
		public int take;

		public Offset(int skip, int take) {
			this.skip = skip;
			this.take = take;
		}
	}


	public class AuthRequest : BaseRequest {
		public string phone;
		public string code;

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Auth/AuthRequest";
	}

	public class OTPRequest : BaseRequest {
		public string phone;
		public string platform;

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Auth/SendOtp";
	}

	public class OperationResult : BaseResponse {
		public string result_code;
		public string message;
	}

	public class UpdateInfoResponse : OperationResponse {
		public List<string> failed_fields;
	}

	public class PostPlayedGame : BaseRequest {
		public int game_id;

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Game/PlayedGame";
	}

	public class PostMetrics : BaseRequest {
		public string session_time;
		public string cps;
		public bool is_bot;
		public string percentage;
		public string click_dur;
		public string gyro_rot;
		public string gyro_accel;
		public string emulator;
		[CanBeNull] public string game_id;
		[CanBeNull] public string reason;
		
		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Metric/Add";
	}

	public class FetchLastPlayedGames : BaseRequest {
		public Offset offset = new Offset(0, 100);

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Game/FetchPlayedGame";
	}

	public class FetchNewGames : BaseRequest {
		public Offset offset = new Offset(0, 100);

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Game/NewGames";
	}

	public class FetchTopGames : BaseRequest {
		public Offset offset = new Offset(0, 100);

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Game/TopGames";
	}

	public class FetchAllGames : BaseRequest {
		public Offset offset = new Offset(0, 100);

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Game/FetchGames";
	}

	public class FetchSingleGame : BaseRequest {
		public int game_id;

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Game/FetchGames";
	}


	public class FetchGamesDataResponse : OperationResponse {
		public List<Game> games;
	}

	public class FetchGamesIdResponse : OperationResponse {
		public List<int> game_ids;
	}

	public class FetchTopPlayers : BaseRequest {
		public enum DateEnum {
			Day,
			Week,
			Month,
			Year
		}

		public string dateString;

		public FetchTopPlayers(DateEnum dateEnum) {
			dateString = dateEnum.ToString();
		}

		public string skip = "0";
		public string take = "10";

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Leaderboard/" + dateString;
	}

	public class FetchTopPlayersResponse : OperationResponse {
		public List<LeaderBoardUser> players;
		public int player_rank;
		public int playerScore;
	}

	public class GetImages : BaseRequest {
		public int[] user_ids;

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/User/Images";
	}

	public class GetImagesResponse : OperationResponse {
		public List<UserImages> imageItems;
	}


	public class FetchOffers : BaseRequest {
		public string skip;
		public string take;

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Offer/Fetch";
	}

	public class FetchTopOffers : BaseRequest {
		public override string GetUrl() =>
			NetHelper.k_ApiServer + "/api/Offer/FetchTopOffers";
	}

	public class FetchTopOffersResponse : OperationResponse {
		public List<TopOffer> top_offers { get; set; }
	}

	public class FetchUserOfferHistory : BaseRequest {
		public override string GetUrl() =>
			NetHelper.k_ApiServer + "/api/User/Offers";
	}

	public class FetchOffersResponse : OperationResponse {
		public List<OfferData> offers;
	}

	public class OwnedOffersResponse : OperationResponse {
		public List<OwnedOfferData> user_offers;
	}

	public class GetOfferCode : BaseRequest {
		public int offer_id;

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Offer/Code";
	}

	public class GetOfferCodeResponse : OperationResponse {
		public string code;
	}

	public class FetchVendors : BaseRequest {
		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Vendor/Fetch";
	}

	public class FetchVendorsResponse : OperationResponse {
		public List<VendorData> vendors;
	}

	public class UserBalanceRequest : BaseRequest {
		public override string GetUrl() =>
			NetHelper.k_ApiServer + "/api/User/Balance";
	}

	public class UserBalanceResponse : OperationResponse {
		public Balance balance;
	}

	public class UpdateUserInfo : BaseRequest {
		public string name;
		public string surname;
		public string email;
		[CanBeNull] public string image;

		public override string PutUrl() =>
			NetHelper.k_ApiServer + "/api/User/UpdateInfo";
	}

	public class Deposit : BaseRequest {
		public int game_id;
		public int coin;
		public string device_os;

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Coin/Deposit";
	}

	public class ExchangeOffer : BaseRequest {
		public int offer_id;

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Coin/Exchange";
	}

	public class TransactionHistory : BaseRequest {
		public string date_from { get; set; }
		public string date_to { get; set; }

		[CanBeNull] public string[] type;
		
		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/Coin/TransactionHistory";
	}

	public class TransactionHistoryResponse : OperationResponse {
		public List<Transaction> transaction_history;
	}


	public class ProfileResponse : OperationResponse {
		public User user_Info;
		public Balance user_Balance;
	}

	public class ProfileGetRequest : BaseRequest {
		public override string GetUrl() =>
			NetHelper.k_ApiServer + "/api/User/Profile";
	}

	public class GetVersion : BaseRequest {
		public string key;

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/KeyStorage/GetValue";
	}

	public class FetchPrizes : BaseRequest {
		public override string GetUrl() =>
			NetHelper.k_ApiServer + "/api/Prize/Fetch";
	}
	
	public class FetchPrizesResponse : OperationResponse {
		public List<PrizeData> list;	
	}

	public class FetchWheelSpinFree : BaseRequest {
		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/LuckyWheel/SpinFree";
	}
	
	public class FetchWheelSpinPaid : BaseRequest {
		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/LuckyWheel/SpinPaid";
	}

	public class FetchWheelSpinResponse : OperationResponse {
		public WheelPrize prize;
	}
	
	public class GetWheelSpinPrizes : BaseRequest {
		public override string GetUrl() =>
			NetHelper.k_ApiServer + "/api/LuckyWheel/List";
	}
	
	public class GetWheelSpinPrizesResponse : OperationResponse {
		public List<WheelPrize> prizes;
	}
	
	public class CheckWheelSpin : BaseRequest {
		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/LuckyWheel/Check";
	}


	public class GetVersionResponse : OperationResponse {
		public string[] value;
	}
	
	public class GetKeyStorage : BaseRequest {
		public string key;

		public override string PostUrl() =>
			NetHelper.k_ApiServer + "/api/KeyStorage/GetValue";
	}

	public class GetKeyStorageResponse : OperationResponse {
		public string[] value;
	}
}