using Microsoft.Extensions.Logging;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class MeasurementService : IMeasurementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MeasurementService> _logger;

        public MeasurementService(IUnitOfWork unitOfWork, ILogger<MeasurementService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<MeasurementResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var measurement = await _unitOfWork.Measurements.GetByIdAsync(id);
                if (measurement == null || measurement.IsDeleted)
                    return BaseResponse<MeasurementResponse>.ErrorResponse("Measurement not found");
                return BaseResponse<MeasurementResponse>.SuccessResponse(MapToResponse(measurement));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting measurement {Id}", id);
                return BaseResponse<MeasurementResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<MeasurementResponse>>> GetAllAsync()
        {
            try
            {
                var list = (await _unitOfWork.Measurements.GetAllAsync())
                    .Where(m => !m.IsDeleted)
                    .Select(MapToResponse)
                    .ToList();
                return BaseResponse<IEnumerable<MeasurementResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting measurements");
                return BaseResponse<IEnumerable<MeasurementResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<MeasurementResponse>>> GetByClientIdAsync(Guid clientId)
        {
            try
            {
                var list = (await _unitOfWork.Measurements.GetByClientIdAsync(clientId))
                    .Select(MapToResponse)
                    .ToList();
                return BaseResponse<IEnumerable<MeasurementResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting measurements for client {ClientId}", clientId);
                return BaseResponse<IEnumerable<MeasurementResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<MeasurementResponse>> CreateAsync(CreateMeasurementRequest request)
        {
            try
            {
                var measurement = new Measurements();
                ApplyRequest(measurement, request);
                var added = await _unitOfWork.Measurements.AddAsync(measurement);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<MeasurementResponse>.SuccessResponse(MapToResponse(added), "Measurement created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating measurement");
                return BaseResponse<MeasurementResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<MeasurementResponse>> UpdateAsync(Guid id, UpdateMeasurementRequest request)
        {
            try
            {
                var measurement = await _unitOfWork.Measurements.GetByIdAsync(id);
                if (measurement == null || measurement.IsDeleted)
                    return BaseResponse<MeasurementResponse>.ErrorResponse("Measurement not found");

                ApplyRequest(measurement, request);
                _unitOfWork.Measurements.Update(measurement);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<MeasurementResponse>.SuccessResponse(MapToResponse(measurement), "Measurement updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating measurement {Id}", id);
                return BaseResponse<MeasurementResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var measurement = await _unitOfWork.Measurements.GetByIdAsync(id);
                if (measurement == null || measurement.IsDeleted)
                    return BaseResponse<bool>.ErrorResponse("Measurement not found");

                measurement.IsDeleted = true;
                measurement.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Measurements.Update(measurement);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Measurement deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting measurement {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        private static void ApplyRequest(Measurements m, CreateMeasurementRequest r)
        {
            m.ClientId = r.ClientId;
            m.Type = r.Type;

            m.Length = r.Length;
            m.Shoulder = r.Shoulder;
            m.Chest = r.Chest;
            m.Waist = r.Waist;
            m.Hip = r.Hip;
            m.Neck = r.Neck;

            m.ArmLength = r.ArmLength;
            m.ArmRound = r.ArmRound;
            m.Bicep = r.Bicep;
            m.Elbow = r.Elbow;
            m.Wrist = r.Wrist;
            m.ArmOpening = r.ArmOpening;

            m.TrouserLength = r.TrouserLength;
            m.Inseam = r.Inseam;
            m.Rise = r.Rise;
            m.Thigh = r.Thigh;
            m.Knee = r.Knee;
            m.Calf = r.Calf;
            m.Ankle = r.Ankle;
            m.Bottom = r.Bottom;

            m.UnderBust = r.UnderBust;
            m.Bust = r.Bust;
            m.NeckDepthFront = r.NeckDepthFront;
            m.NeckDepthBack = r.NeckDepthBack;

            m.SidePocket = r.SidePocket;
            m.FrontPocket = r.FrontPocket;
            m.BackPocket = r.BackPocket;
            m.BreastPocket = r.BreastPocket;
            m.InnerPocket = r.InnerPocket;

            m.CollarType = r.CollarType;
            m.CuffType = r.CuffType;
            m.FitType = r.FitType;
            m.TrouserType = r.TrouserType;
            m.LapelStyle = r.LapelStyle;
            m.NeckStyle = r.NeckStyle;

            m.Height = r.Height;
            m.Weight = r.Weight;

            m.Notes = r.Notes;
        }

        private static MeasurementResponse MapToResponse(Measurements m) => new()
        {
            Id = m.Id,
            ClientId = m.ClientId,
            Type = m.Type,

            Length = m.Length,
            Shoulder = m.Shoulder,
            Chest = m.Chest,
            Waist = m.Waist,
            Hip = m.Hip,
            Neck = m.Neck,

            ArmLength = m.ArmLength,
            ArmRound = m.ArmRound,
            Bicep = m.Bicep,
            Elbow = m.Elbow,
            Wrist = m.Wrist,
            ArmOpening = m.ArmOpening,

            TrouserLength = m.TrouserLength,
            Inseam = m.Inseam,
            Rise = m.Rise,
            Thigh = m.Thigh,
            Knee = m.Knee,
            Calf = m.Calf,
            Ankle = m.Ankle,
            Bottom = m.Bottom,

            UnderBust = m.UnderBust,
            Bust = m.Bust,
            NeckDepthFront = m.NeckDepthFront,
            NeckDepthBack = m.NeckDepthBack,

            SidePocket = m.SidePocket,
            FrontPocket = m.FrontPocket,
            BackPocket = m.BackPocket,
            BreastPocket = m.BreastPocket,
            InnerPocket = m.InnerPocket,

            CollarType = m.CollarType,
            CuffType = m.CuffType,
            FitType = m.FitType,
            TrouserType = m.TrouserType,
            LapelStyle = m.LapelStyle,
            NeckStyle = m.NeckStyle,

            Height = m.Height,
            Weight = m.Weight,

            Notes = m.Notes,

            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        };
    }
}
