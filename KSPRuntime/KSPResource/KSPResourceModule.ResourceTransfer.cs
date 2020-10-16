using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPResource {
    enum TransferStatus {
        Failed,
        Finished,
        Transferring
    }

    public partial class KSPResourceModule {
        [KSClass("ResourceTransfer")]
        public class ResourceTransfer : IFixedUpdateObserver {
            private const float ResourceSharePerUpdate = 0.005f;

            private readonly ResourceDefinitionAdapter resource;
            private readonly KSPVesselModule.PartAdapter[] transferFrom;
            private readonly KSPVesselModule.PartAdapter[] transferTo;
            private readonly double amount;
            private double transferredAmount;
            private TransferStatus status;
            private string statusMessage;

            public ResourceTransfer(IKSPContext context, ResourceDefinitionAdapter resource,
                KSPVesselModule.PartAdapter[] transferFrom,
                KSPVesselModule.PartAdapter[] transferTo, double amount) {
                this.resource = resource;
                this.transferFrom = transferFrom;
                this.transferTo = transferTo;
                this.amount = amount;
                transferredAmount = 0;
                status = TransferStatus.Transferring;
                statusMessage = "";
                context.AddFixedUpdateObserver(this);
            }

            [KSField] public ResourceDefinitionAdapter Resource => resource;

            [KSField] public double Goal => amount;

            [KSField] public double Transferred => transferredAmount;

            [KSField] public string Status => status.ToString();

            [KSField] public string StatusMessage => statusMessage;

            [KSMethod]
            public void Abort() => MarkFailed("Transfer aborted");
            
            public void OnFixedUpdate(double deltaTime) {
                if (status != TransferStatus.Transferring) {
                    return;
                }

                if (!AllPartsAreConnected()) {
                    return;
                }

                if (!CanTransfer()) {
                    return;
                }

                WorkTransfer(deltaTime);
            }

            private void WorkTransfer(double deltaTime) {
                var transferGoal = CalculateTransferGoal();

                double pulledAmount = PullResources(transferGoal, deltaTime);

                PutResources(pulledAmount);

                transferredAmount += pulledAmount;

                if (status == TransferStatus.Transferring) {
                    statusMessage = $"Transferred: {transferredAmount}";
                }
            }

            private void PutResources(double pulledAmount) {
                var retries = 0;
                var evenShare = pulledAmount / transferTo.Length;

                var remaining = pulledAmount;
                while (remaining > 0.0001) {
                    if (retries > 10) {
                        MarkFailed("Error in putting resource with " + remaining + " remaining.");
                        break;
                    }

                    foreach (var part in transferTo) {
                        var resource = part.part.Resources.Get(this.resource.resourceDefinition.id);
                        if (resource == null) continue;

                        var transferAmount = Math.Min(remaining, evenShare);

                        remaining += part.part.TransferResource(resource.info.id, transferAmount);
                    }

                    retries++;
                }
            }

            private double PullResources(double transferGoal, double deltaTime) {
                double toReturn = 0.0;
                var availableResources = CalculateAvailableResource();
                foreach (var part in transferFrom) {
                    var resource = part.part.Resources.Get(this.resource.resourceDefinition.id);
                    if (resource == null) continue;

                    var thisPartsPercentage = resource.amount / availableResources;

                    // Throttle the transfer
                    var thisPartsShare = transferGoal * thisPartsPercentage;
                    var thisPartsRate = resource.maxAmount * ResourceSharePerUpdate * deltaTime / 0.02;

                    // The amount you pull must be negative 
                    thisPartsShare = -Math.Min(thisPartsShare, thisPartsRate);
                    // the amount is subject to floating point lameness, if we round it here it is not material to the request but should make the numbers work out nicer.
                    thisPartsShare = Math.Round(thisPartsShare, 5);

                    toReturn += part.part.TransferResource(this.resource.resourceDefinition.id, thisPartsShare);
                }

                return toReturn;
            }

            private bool CanTransfer() {
                if (!DestinationReady()) return false;
                if (!SourceReady()) return false;

                if (amount >= 0) {
                    if (Math.Abs(amount - transferredAmount) < 0.00001) {
                        MarkFinished();
                        return false;
                    }
                }

                // We aren't finished yet!
                return true;
            }

            private bool SourceReady() {
                var sourceAvailable = CalculateAvailableResource();
                if (Math.Abs(sourceAvailable) < 0.0001) {
                    // Nothing to transfer
                    if (amount < 0) {
                        MarkFinished();
                    } else {
                        MarkFailed("Source is out of " + resource.resourceDefinition.name);
                    }

                    return false;
                }

                return true;
            }


            private bool DestinationReady() {
                var destinationAvailableCapacity = CalculateAvailableSpace();
                if (Math.Abs(destinationAvailableCapacity) < 0.0001) {
                    // No room at the inn
                    if (amount < 0) {
                        MarkFinished();
                    } else {
                        MarkFailed("Destination is out of space");
                    }

                    return false;
                }

                return true;
            }

            private void MarkFailed(string message) {
                statusMessage = message;
                status = TransferStatus.Failed;
            }

            private void MarkFinished() {
                transferredAmount = Math.Round(transferredAmount, 5);
                statusMessage = $"Transferred: {transferredAmount}";
                status = TransferStatus.Finished;
            }

            private double CalculateTransferGoal() {
                var destinationAvailableCapacity = CalculateAvailableSpace();

                if (amount >= 0) {
                    var rawGoal = amount - transferredAmount;
                    return Math.Min(destinationAvailableCapacity, rawGoal);
                }

                return destinationAvailableCapacity;
            }

            private double CalculateAvailableSpace() {
                var resources = new List<PartResource>();
                // GetAll no longer returns a list, but instead fills an existing list
                foreach (var part in transferTo) {
                    part.part.Resources.GetAll(resources, resource.resourceDefinition.id);
                }

                return resources.Sum(r => r.maxAmount - r.amount);
            }

            private double CalculateAvailableResource() {
                var resources = new List<PartResource>();
                // GetAll no longer returns a list, but instead fills an existing list
                foreach (var part in transferFrom) {
                    part.part.Resources.GetAll(resources, resource.resourceDefinition.id);
                }

                return resources.Sum(r => r.amount);
            }

            private bool AllPartsAreConnected() {
                var fromFlightId = GetVesselId(transferFrom);
                var toFlightId = GetVesselId(transferTo);

                if (!fromFlightId.HasValue) {
                    MarkFailed("Not all From parts are connected");
                    return false;
                }

                if (!toFlightId.HasValue) {
                    MarkFailed("Not all To parts are connected");
                    return false;
                }

                if (fromFlightId != toFlightId) {
                    MarkFailed("To and From are not connected");
                    return false;
                }

                return true;
            }

            private Guid? GetVesselId(KSPVesselModule.PartAdapter[] parts) {
                if (parts.Length == 0)
                    return null;
                var vessel = parts[0].part.vessel;
                if (parts.All(p => p.part.vessel == vessel)) return vessel.id;

                // Some parts are from a different vessel? bail
                return null;
            }
        }
    }
}
