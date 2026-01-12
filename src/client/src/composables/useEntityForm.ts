import { type Ref, ref } from 'vue'
import type { Entity } from '@/types/entity.ts'
import { err, ok, type Result } from '@/types/result.ts'
import type { PropertyRules } from '@/validation/types/propertyRules.ts'
import { createFormValidation, type FormValidation } from '@/validation/types/formValidation.ts'

interface UseEntityFormParameters<
  TRequest extends object,
  TFormState extends TRequest,
  TEntity extends Entity
> {
  validationRules:
    | PropertyRules<TFormState>
    | ((formState: Ref<TFormState>) => PropertyRules<TFormState>)
  formStateInitializeFn: ((entity?: TEntity) => Ref<TFormState>) | (() => Ref<TFormState>)
  entity?: TEntity
  createPersistentEntityFn?: (
    request: Ref<TRequest>,
    validation: FormValidation<TRequest>,
    progressRef?: Ref<number>
  ) => Promise<Result>
  updatePersistentEntityFn?: (
    id: string,
    request: Ref<TRequest>,
    validation: FormValidation<TRequest>,
    progressRef?: Ref<number>
  ) => Promise<Result>
  onSubmitted?: () => void
  useProgress?: boolean
}

interface UseEntityFormReturn<TFormState extends object> {
  state: Ref<TFormState>
  val: FormValidation<TFormState>
  isSubmitting: Ref<boolean>
  submit: () => Promise<Result>
  reset: () => void
  progress: Ref<number>
}

/**
 * Composable for managing entity forms with validation, submission, and reset handling.
 *
 * @param validationRules The validation rules for the given TFormState.
 * @param entity The entity being edited, if it exists.
 * @param formStateInitializeFn Function to map a `TEntity` instance to a `Ref<TEntity>`.
 * @param createPersistentFn Function to create a new persistent entity in the store. See `useCreatePersistentItemFn`.
 * @param updatePersistentFn Function to update an existing persistent entity in the store. See
 *   `useUpdatePersistentItemFn`.
 * @param onSubmitted Callback function to be called after successful submission.
 * @param useProgress Will provide a progress `Ref<number>` to the create/update functions if `true`. Only applies to
 *   xhr requests.
 *
 * @typeParam TRequest – The type of the request object used for creating/updating the entity.
 * @typeParam TFormState – The type of the form state, extending TRequest.
 * @typeParam TEntity – The type of the entity being edited, extending Entity.
 *
 */
export const useEntityForm = <
  TRequest extends object,
  TFormState extends TRequest,
  TEntity extends Entity
>({
  validationRules,
  entity,
  formStateInitializeFn,
  createPersistentEntityFn,
  updatePersistentEntityFn,
  onSubmitted = () => {},
  useProgress = false
}: UseEntityFormParameters<TRequest, TFormState, TEntity>): UseEntityFormReturn<TFormState> => {
  let state: Ref<TFormState>
  if (formStateInitializeFn.length == 1) state = formStateInitializeFn(entity)
  else state = formStateInitializeFn()

  const rules =
    typeof validationRules === 'function' ? validationRules(state) : validationRules
  const val: FormValidation<TFormState> = createFormValidation(state, rules)
  const isSubmitting = ref(false)
  const progress = ref(0)

  const submit = async (): Promise<Result> => {
    isSubmitting.value = true
    const result = await apiAction(entity)
    isSubmitting.value = false
    if (!result.isSuccess) return err()
    reset()
    if (onSubmitted) onSubmitted()
    return ok()
  }

  const apiAction = async (entity?: TEntity) => {
    let result: Result | undefined = undefined
    if (entity !== undefined && updatePersistentEntityFn) {
      if (useProgress) result = await updatePersistentEntityFn(entity.id, state, val, progress)
      else result = await updatePersistentEntityFn(entity.id, state, val)
    } else if (entity === undefined && createPersistentEntityFn) {
      if (useProgress) result = await createPersistentEntityFn(state, val, progress)
      else result = await createPersistentEntityFn(state, val)
    }
    if (result === undefined) throw new Error('Missing store method implementation for action.')
    return result
  }

  const reset = () => {
    state.value = formStateInitializeFn(entity).value
    val.reset()
  }

  return { state, val, isSubmitting, submit, reset, progress }
}
