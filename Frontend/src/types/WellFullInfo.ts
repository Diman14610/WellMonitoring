import WellWithContractor from "./WellWithContractor";

export default interface WellFullInfo extends WellWithContractor {
  wellId: number
  depth: number
  dateTime: string
  telemetryId: number
}