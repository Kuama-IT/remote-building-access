import { z } from "zod";
import { OutdatedNukiCredentialResponseDtoSchema } from "../../nuki-credentials/api/nuki-credential.schemas";
import { SmartLockProviderEnumSchema } from "../../shared/smart-lock-provider.schemas";

export const SmartLockKeyResponseDtoSchema = z.object({
  id: z.string(),
  validFromDate: z.date(),
  validUntilDate: z.date(),
  password: z.string().nullable(),
});

export type SmartLockKeyResponseDto = z.infer<
  typeof SmartLockKeyResponseDtoSchema
>;

export const SmartLockKeyListResponseDtoSchema = z.object({
  smartLockKeys: z.array(SmartLockKeyResponseDtoSchema),
  outdatedCredentials: z.array(OutdatedNukiCredentialResponseDtoSchema),
});

export type SmartLockKeyListResponseDto = z.infer<
  typeof SmartLockKeyListResponseDtoSchema
>;

export const CreateSmartLockKeyRequestDtoSchema = z.object({
  smartLockId: z.string(),
  validFromDate: z.date(),
  validUntilDate: z.date(),
  credentialId: z.number(),
  smartLockProvider: SmartLockProviderEnumSchema,
});

export type CreateSmartLockKeyRequestDto = z.infer<
  typeof CreateSmartLockKeyRequestDtoSchema
>;
