export interface UserSummary {
  id: number;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  tenantId: number;
  departmentId: number | null;
  staffRoleId: number;
  staffRoleName: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
  user: UserSummary;
}

export interface IdNameDto {
  id: number;
  name: string;
}

export interface EquipmentApplicationLookupDto extends IdNameDto {
  equipmentTypeId: number;
}

export interface LookupBundleDto {
  departments: IdNameDto[];
  equipmentTypes: IdNameDto[];
  equipmentApplications: EquipmentApplicationLookupDto[];
  maintenanceTypes: IdNameDto[];
}

export interface EquipmentDto {
  id: number;
  departmentId: number;
  departmentName: string;
  equipmentTypeId: number;
  equipmentTypeName: string;
  equipmentApplicationId: number;
  equipmentApplicationName: string;
  name: string;
  brand: string;
  model: string;
  year: number;
  serialNumber: string;
  ecriCode: string;
  commissionedOn: string;
  decommissionedOn: string | null;
  publicId: string;
  hasOpenMaintenance: boolean;
}

export interface UpsertEquipmentRequest {
  id: number;
  departmentId: number;
  equipmentTypeId: number;
  equipmentApplicationId: number;
  name: string;
  brand: string;
  model: string;
  year: number;
  serialNumber: string;
  ecriCode: string;
  commissionedOn: string;
  decommissionedOn: string | null;
}

export interface MaintenanceEventDto {
  id: number;
  equipmentId: number;
  maintenanceTypeId: number;
  maintenanceTypeName: string;
  requestedAt: string;
  completedAt: string | null;
  requestedByName: string;
  noteLines: string[];
}
