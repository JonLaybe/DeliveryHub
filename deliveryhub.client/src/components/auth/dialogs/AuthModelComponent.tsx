import { useEffect, useState, type FC } from "react";
import Modal from "react-modal";
import './AuthModelComponent.scss';
import { useForm } from "react-hook-form";
import type { LoginRequestDto, Password } from "../../../models/auth-service/LoginRequestDto";
import { loginAsync } from "../../../services/auth-service/AuthService";

interface AuthModelProps {
    value: boolean;
    onChange: (newValue: boolean) => void;
}

const AuthModelComponent: FC<AuthModelProps> = ({ value, onChange }) => {

    const [modalIsOpen, setModalIsOpen] = useState(value);
    const {
        register,
        reset,
        handleSubmit,
        formState: { isSubmitting, isSubmitSuccessful }
    } = useForm({
        defaultValues: {
            email: '',
            password: '' as Password,
        }
    });

    useEffect(() => {
        setModalIsOpen(value);
    }, [value]);

    useEffect(() => {
        if (isSubmitSuccessful)
            reset();
    }, [isSubmitSuccessful]);

    const openModal = () => {
        setModalIsOpen(true);
        onChange(true);
    };
    const closeModal = () => {
        setModalIsOpen(false);
        onChange(false);
    };

    const onLogin = async (dataLoginRequest: LoginRequestDto) => {
        await loginAsync(dataLoginRequest).then(() => closeModal())
        .catch((e) => {
            console.log('>Login failed', e);
            throw 'Login failed';
        });
    };

    return (
        <div className="container_model_auth">
            <Modal className="custom_model" overlayClassName="custom_model_overlay" isOpen={modalIsOpen} onRequestClose={closeModal}>
                <div className="contect_model_auth">
                    <div className="contect_model_auth__header">
                        <h1 className="contect__name_chapter">
                            <span className="default_name_chapter body">DeliveryHub</span>
                            <span className="default_name_chapter contect__name_chapter prefix"> ID</span>
                        </h1>
                    </div>
                    <div className="contect_model_auth__main">
                        <form onSubmit={handleSubmit((data) => onLogin(data))}>
                            <label className="default_text">Логин</label>
                            <input {...register('email')} className="default_text" type="email" maxLength={250} />
                            <label className="default_text">Пароль</label>
                            <input {...register('password')} className="default_text" type="password" />
                            <input type="submit" disabled={isSubmitting} value="Войти" />
                        </form>
                    </div>
                </div>
            </Modal >
        </div >
    )
};

export default AuthModelComponent;