import { useEffect, useState, type FC } from "react";
import Modal from "react-modal";
import { useForm } from "react-hook-form";
import './PromoCodePanel.scss';
import type { ApplyResponseModel } from "../../models/discount-service/ApplyResponseModel";

export type PromoApplyResult = {
  success: boolean;
  code?: string;
  appliedAmount?: number;
  discountType?: number;
  discountUsageId?: number;
  message?: string;
};

export type PromoApplyRequest = {
  Code: string;
  OrderAmount: number;
}

interface PromoCodePanelProps {
  value: boolean;
  onApply: (code: string) => Promise<PromoApplyResult>;
}

const PromoCodePanel: FC<PromoCodePanelProps> = ({ value, onApply}) => {
  const [modalIsOpen, setModalIsOpen] = useState<boolean>(value);
  const {
    register,
    reset,
    handleSubmit,
    formState: { isSubmitting, isSubmitSuccessful }
  } = useForm<{ code: string }>({
    defaultValues: { code: '' }
  });

  const [error, setError] = useState<string | null>(null);
  const [applied, setApplied] = useState<{ code: string; appliedAmount?: number; discountType?: number } | null>(null);

  useEffect(() => setModalIsOpen(value), [value]);

  useEffect(() => {
    if (isSubmitSuccessful) reset({ code: '' });
  }, [isSubmitSuccessful]);

  const openModal = () => {
    setModalIsOpen(true);
  };

  const closeModal = () => {
    setModalIsOpen(false);
  };

  const onSubmitPromo = async (data: { code: string }) => {
    const res = await onApply(data.code);
    if (res?.success) {
      setApplied({ code: res.code ?? data.code, appliedAmount: res.appliedAmount, discountType: res.discountType });
      setError(null);
      closeModal();
    } else {
      setError(res?.message ?? 'Промокод недействителен');
    }
  };

  return (
    <div className="container_model_promo">
      <button className="default-button" onClick={openModal} aria-label="Открыть панель промокода">
        Применить промокод
      </button>

      <Modal
        className="custom_model"
        overlayClassName="custom_model_overlay"
        isOpen={modalIsOpen}
        onRequestClose={closeModal}
        ariaHideApp={false}
      >
        <div className="contect_model_auth">
          <div className="contect_model_auth__header">
            <h1 className="contect__name_chapter">
              <span className="default_name_chapter body">DeliveryHub</span>
              <span className="default_name_chapter contect__name_chapter prefix"> Promo</span>
            </h1>
          </div>

          <div className="contect_model_auth__main">
            <form onSubmit={handleSubmit(onSubmitPromo)}>
              <label className="default_text" htmlFor="promo-code-input">Промокод</label>
              <input
                id="promo-code-input"
                {...register('code')}
                className="default_text"
                type="text"
                maxLength={60}
              />
              <div className="promo_actions">
                <button type="submit" className="apply_button" disabled={isSubmitting}>
                  {isSubmitting ? 'Применяю…' : 'Применить'}
                </button>
                <button type="button" className="close_button" onClick={closeModal} disabled={isSubmitting}>
                  Закрыть
                </button>
              </div>
            </form>
            {error && <div className="error">{error}</div>}
            {applied && !error && (<div className="success">Применено: {applied.code} {typeof applied.appliedAmount === 'number' && (<span> • Скидка: {applied.discountType === 0 ? `${applied.appliedAmount}%` : `${applied.appliedAmount}`}</span>)}</div>)}
          </div>
        </div>
      </Modal>
    </div>
  );
};

export default PromoCodePanel;