import type { Entity } from '@/types/entity.ts'
import type { ApiResponse } from '@/api/apiResponse.ts'
import type { ToastServiceMethods } from 'primevue'
import { getEntity } from '@/utils/arrayUtils.ts'
import { err, ok, type Result } from '@/types/result.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import type { Ref } from 'vue'
import type { FormValidation } from '@/validation/types/formValidation.ts'

export const useCreatePersistentItemFn =
  <TRequest extends object>(
    apiFunction: (request: TRequest) => Promise<ApiResponse<TRequest>>,
    onSuccess: (id: string, request: TRequest) => void,
    toast?: ToastServiceMethods
  ) =>
  async (formState: Ref<TRequest>, validation: FormValidation<TRequest>): Promise<Result> => {
    if (!validation.validate()) return err()
    const response = await apiFunction(formState.value)
    if (toast && tryHandleUnsuccessfulResponse(response, toast, validation)) return err()
    else if (!response.isSuccess()) return err()
    onSuccess(response.getCreatedId(), formState.value)
    return ok()
  }

export const useUpdatePersistentItemFn =
  <TRequest extends object, TEntity extends Entity>(
    entities: Ref<TEntity[]>,
    apiFunction: (id: string, data: TRequest) => Promise<ApiResponse<TRequest>>,
    onSuccess: (request: TRequest, entity: TEntity) => void,
    toast?: ToastServiceMethods
  ) =>
  async (
    id: string,
    formState: Ref<TRequest>,
    validation: FormValidation<TRequest>
  ): Promise<Result> => {
    const entity = getEntity(entities.value, id)
    if (!entity) return err()
    if (!validation.validate()) return err()
    const response = await apiFunction(id, formState.value)
    if (toast && tryHandleUnsuccessfulResponse(response, toast, validation)) return err()
    else if (!response.isSuccess()) return err()
    onSuccess(formState.value, entity)
    return ok()
  }

export const useRemovePersistentItemFn =
  <TEntity extends Entity>(
    entities: Ref<TEntity[]>,
    apiFunction: (id: string) => Promise<ApiResponse>,
    onSuccess: (entity: TEntity) => void,
    toast?: ToastServiceMethods
  ) =>
  async (id: string): Promise<Result> => {
    console.log(id)
    const entity = getEntity(entities.value, id)
    console.log(entity)
    if (!entity) return err()
    const response = await apiFunction(id)
    if (toast && tryHandleUnsuccessfulResponse(response, toast)) return err()
    else if (!response.isSuccess()) return err()
    onSuccess(entity)
    return ok()
  }
