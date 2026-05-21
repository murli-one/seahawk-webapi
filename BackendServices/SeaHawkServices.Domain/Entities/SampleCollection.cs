using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Data
{
    //public class SampleCollection
    //{
    //    public Company Company { get; set; }
    //    public int? CompanyId { get; set; }
    //    public int Id { get; set; }
    //    public string? VesselName { get; set; }
    //    public string? IMONumber { get; set; }
    //    public string? NoOfSamples { get; set; }
    //    public string? RequestorName { get; set; }
    //    public string? RequestorEmail { get; set; }
    //    public string? AgentEmail { get; set; }
    //    public string? EmailCC1 { get; set; }
    //    public string? EmailCC2 { get; set; }
    //    public string? EmailCC3 { get; set; }
    //    public string? TelephoneNo { get; set; }
    //    public string? CountryName { get; set; }
    //    public string? ContactPerson { get; set; }
    //    public string? ContactNo { get; set; }
    //    public string? PickupAddress { get; set; }
    //    public string? City { get; set; }
    //    public string? Town { get; set; }
    //    public string? PostalCode { get; set; }
    //    public DateTime? PreferredDate { get; set; }
    //    public string? PreferredTime { get; set; }
    //    [NotMapped]
    //    public HourEnum HourEnum { get; set; }
    //    [NotMapped]
    //    public MinuteEnum MinuteEnum { get; set; }
    //    [NotMapped]
    //    public string? CompanyName { get; set; }
    //    public DateTime? CreatedDate { get; set; }
    //    public ApplicationUser CreatedUser { get; set; }
    //    public string? CreatedUserId { get; set; }
    //    public DateTime? UpdatedDate { get; set; }
    //    public string? UpdatedByUser { get; set; }
    //    public string? AWBNumber { get; set; }
    //    public string? CurrentStatus { get; set; }
    //    public string? LastUpdated { get; set; }
    //    public string? EstimatedDeliveryDate { get; set; }
    //    public string? ShipmentType { get; set; }
    //    public string? ShippingDate { get; set; }
    //    public string? ShipperName { get; set; }
    //    public string? ConsigneeName { get; set; }
    //    public string? Pieces { get; set; }
    //    public string? Weight { get; set; }
    //    public string? ShipmentDesc { get; set; }
    //    public string? ShipperCity { get; set; }
    //    public string? ShipperPostalCode { get; set; }
    //    public string? ShipperCountryCode { get; set; }
    //    public string? ConsigneeCity { get; set; }
    //    public string? ConsigneePostalCode { get; set; }
    //    public string? ConsigneeCountryCode { get; set; }
    //    public string? DuplicateWaybill { get; set; }
    //    public string? TrackingNumberUniqueIdentifier { get; set; }
    //    public string? StatusDetailCreationTime { get; set; }
    //    public string? StatusDetailCode { get; set; }
    //    public string? StatusDetailDescription { get; set; }
    //    public string? StatusDetailLocationCity { get; set; }
    //    public string? StatusDetailLocationCountryCode { get; set; }
    //    public string? StatusDetailLocationCountryName { get; set; }
    //    public string? StatusDetailLocationResidential { get; set; }
    //    public string? OtherIdentifiersType { get; set; }
    //    public string? OtherIdentifiersValue { get; set; }
    //    public string? PackageWeightUnits { get; set; }
    //    public string? PackageWeightValue { get; set; }
    //    public string? PackageDimensionsLength { get; set; }
    //    public string? PackageDimensionsWidth { get; set; }
    //    public string? PackageDimensionsHeight { get; set; }
    //    public string? PackageDimensionsUnits { get; set; }
    //    public string? ShipmentWeightUnits { get; set; }
    //    public string? ShipmentWeightValue { get; set; }
    //    public string? PackagingType { get; set; }
    //    public string? PackageCount { get; set; }
    //    public string? SpecialHandlingsType { get; set; }
    //    public string? SpecialHandlingsDescription { get; set; }
    //    public string? SpecialHandlingsPaymentType { get; set; }
    //    public string? ShipperStateCode { get; set; }
    //    public string? ShipperCountry { get; set; }
    //    public string? ShipperResidential { get; set; }
    //    public string? ConsigneeStateCode { get; set; }
    //    public string? ConsigneeCountry { get; set; }
    //    public string? ConsigneeResidential { get; set; }
    //    public string? ActualDeliveryCity { get; set; }
    //    public string? ActualDeliveryCountryCode { get; set; }
    //    public string? ActualDeliveryCountry { get; set; }
    //    public string? ActualDeliveryResidential { get; set; }
    //    public string? DeliveryLocationDescription { get; set; }
    //    public string? DeliveryAttempts { get; set; }
    //    public string? DeliverySignatureName { get; set; }
    //    public string? OriginServiceAreaCode { get; set; }
    //    public string? OriginServiceAreaDescription { get; set; }
    //    public string? DestinationServiceAreaCode { get; set; }
    //    public string? DestinationServiceAreaDescription { get; set; }
    //    public string? ShipperAccountNumber { get; set; }
    //    public string? GlobalProductCode { get; set; }
    //    public string? DlvyNotificationFlag { get; set; }
    //    public string? ShipperReferenceID { get; set; }
    //    public string? WeightUnit { get; set; }
    //    public string? DeliveryDate { get; set; }
    //}
}
