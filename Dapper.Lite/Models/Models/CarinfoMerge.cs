using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Lite;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [Table("CARINFO_MERGE")]
    public partial class CarinfoMerge
    {

        /// <summary>
        /// 
        /// </summary>
        [Key]
        [Column("ID")]
        public int? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("MODIFY_TIME")]
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("LICENSE_NO")]
        public string LicenseNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CAR_PLATE_COLOR")]
        public string CarPlateColor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("BRAND")]
        public string Brand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("MODEL")]
        public string Model { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CAR_TYPE")]
        public string CarType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("PASSENGER_LEVEL")]
        public string PassengerLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CAR_COLOR")]
        public string CarColor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ENG_NO")]
        public string EngNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("FRAME_NO")]
        public string FrameNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CAR_IDENTITY_CODE")]
        public string CarIdentityCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("SEAT_NO")]
        public decimal? SeatNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CAR_TONNAGE")]
        public decimal? CarTonnage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("FUEL_TYPE")]
        public string FuelType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ENG_POWER")]
        public decimal? EngPower { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("LEAVE_FACTORY_TIME")]
        public DateTime? LeaveFactoryTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("BUY_CAR_TIME")]
        public DateTime? BuyCarTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("SETTLE_TIME")]
        public DateTime? SettleTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("WHEELBASE")]
        public int? Wheelbase { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CAR_LENGTH")]
        public int? CarLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CAR_HEIGHT")]
        public int? CarHeight { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CAR_WIDTH")]
        public int? CarWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("DRIVING_WAY")]
        public string DrivingWay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("TRANSFORM_LICENSE_NO")]
        public string TransformLicenseNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("OWNER_NAME")]
        public string OwnerName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("BUSINESS_LICENSE_NO")]
        public string BusinessLicenseNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("GRANT_ORG")]
        public string GrantOrg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("BEGIN_TIME")]
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("END_TIME")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("BUSINESS_SCOPE")]
        public string BusinessScope { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CAR_OPERATE_SITUATION")]
        public string CarOperateSituation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CAR_TECHNOLOGY_LEVEL")]
        public string CarTechnologyLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("DRIVE_RECORDER")]
        public int? DriveRecorder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("LOCATOR")]
        public int? Locator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("FUEL_EXAM_TIME")]
        public DateTime? FuelExamTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("FIRST_GRANT_TIME")]
        public DateTime? FirstGrantTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ANCHORED")]
        public string Anchored { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ADMINISTRATIVE_DIVISION_CODE")]
        public string AdministrativeDivisionCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ORG_CODE")]
        public string OrgCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("OWNER_ADDRESS")]
        public string OwnerAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ECONOMIC_TYPE")]
        public string EconomicType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("LEGAL_REPRESENTATIVE")]
        public string LegalRepresentative { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ID_CARD_NO")]
        public string IdCardNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("PHONE")]
        public string Phone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("TELEPHONE")]
        public string Telephone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("OWNER_TYPE")]
        public string OwnerType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("OWNER_ABBREVIATION")]
        public string OwnerAbbreviation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CHECK_COMPANY")]
        public string CheckCompany { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("VEHICLE_NO")]
        public string VehicleNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CHECK_PERSON")]
        public string CheckPerson { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("MANAFACTURE")]
        public string Manafacture { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("PRODUCT_NAME")]
        public string ProductName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("VHCL_COMPANY")]
        public string VhclCompany { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("VHCL_TYPE")]
        public string VhclType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CHASSIS_TYPE")]
        public string ChassisType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("DRIVE_TYPE")]
        public string DriveType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("TYRE_SIZE")]
        public string TyreSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ENGINE_TYPE1")]
        public string EngineType1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("TOTAL_MASS")]
        public decimal? TotalMass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("TRALIER_MASS")]
        public decimal? TralierMass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("WHEEL_MASS")]
        public decimal? WheelMass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("TRUCK_VOL")]
        public decimal? TruckVol { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("SEAT_SUM")]
        public int? SeatSum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("LENGTH")]
        public int? Length { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("WIDTH")]
        public int? Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("HIGH")]
        public int? High { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("BOX_LENGTH")]
        public int? BoxLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("BOX_WIDTH")]
        public int? BoxWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("BOX_HIGH")]
        public int? BoxHigh { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CURB_WEIGHT")]
        public int? CurbWeight { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("SIZE_EXCEED")]
        public decimal? SizeExceed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("BOARD_EXCEED")]
        public decimal? BoardExceed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CURB_WEIGHT_EXCEED")]
        public decimal? CurbWeightExceed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CHECK_COM_OPTION")]
        public string CheckComOption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("MANAGER_COM_PERSON")]
        public string ManagerComPerson { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("MANAGER_COM_OPTION")]
        public string ManagerComOption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("TEST_DATE")]
        public DateTime? TestDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("IS_TRANS")]
        public string IsTrans { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("DISTRICT_ID")]
        public string DistrictId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ENGINE_TYPE")]
        public string EngineType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("TELEPHONE_1")]
        public string Telephone1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CHECK_STATE")]
        public string CheckState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("SEAT_EXCEED")]
        public decimal? SeatExceed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("VHCL_COLOR")]
        public string VhclColor { get; set; }

    }
}
