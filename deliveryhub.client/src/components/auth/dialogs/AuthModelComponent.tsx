import { useEffect, useState, type FC } from "react";
import Modal from "react-modal";
import './AuthModelComponent.scss';
import { useForm } from "react-hook-form";
import type { LoginRequestDto, Password } from "../../../models/auth-service/LoginRequestDto";
import {
    getLoginErrorMessage,
    getRegisterErrorMessage,
    loginAsync,
    registerAsync
} from "../../../services/auth-service/AuthService";
import { toast } from "react-hot-toast";

interface AuthModelProps {
    value: boolean;
    onChange: (newValue: boolean) => void;
}

type AuthForm = {
    email: string;
    password: Password;
    confirmPassword?: Password;
};

const AuthModelComponent: FC<AuthModelProps> = ({ value, onChange }) => {
    const [modalIsOpen, setModalIsOpen] = useState(value);
    const [mode, setMode] = useState<"login" | "register">("login");

    const {
        register,
        reset,
        handleSubmit,
        formState: { isSubmitting, isSubmitSuccessful }
    } = useForm<AuthForm>({
        defaultValues: {
            email: '',
            password: '' as Password,
            confirmPassword: '' as Password,
        }
    });

    useEffect(() => {
        setModalIsOpen(value);
    }, [value]);

    useEffect(() => {
        if (isSubmitSuccessful) {
            reset();
        }
    }, [isSubmitSuccessful, reset]);

    const closeModal = () => {
        setModalIsOpen(false);
        onChange(false);
        setMode("login");
        reset();
    };

    const switchMode = (newMode: "login" | "register") => {
        setMode(newMode);
        reset();
    };

    const onLogin = async (dataLoginRequest: LoginRequestDto) => {
        try {
            await loginAsync(dataLoginRequest);
            closeModal();
            toast.success("Вы успешно вошли.");
        } catch (error) {
            console.log('>Login failed', error);
            toast.error(getLoginErrorMessage(error));
        }
    };

    const onRegister = async (data: AuthForm) => {
        if ((data.confirmPassword ?? '') !== data.password) {
            toast.error("Пароли не совпадают.");
            return;
        }

        try {
            await registerAsync({ email: data.email, password: data.password });
            closeModal();
            toast.success("Регистрация прошла успешно.");
        } catch (error) {
            console.log('>Register failed', error);
            toast.error(getRegisterErrorMessage(error));
        }
    };

    const onSubmit = async (data: AuthForm) => {
        if (mode === "login") {
            await onLogin({
                email: data.email,
                password: data.password
            });
        } else {
            await onRegister(data);
        }
    };

    return (
        <div className="container_model_auth">
            <Modal
                className="custom_model"
                overlayClassName="custom_model_overlay"
                isOpen={modalIsOpen}
                onRequestClose={closeModal}
            >
                <div className="contect_model_auth">
                    <div className="contect_model_auth__header">
                        <h1 className="contect__name_chapter">
                            <span className="default_name_chapter body">DeliveryHub</span>
                            <span className="default_name_chapter contect__name_chapter prefix"> ID</span>
                        </h1>

                        <div className="auth_mode_switcher">
                            <button
                                type="button"
                                className={`auth_mode_button ${mode === "login" ? "active" : ""}`}
                                onClick={() => switchMode("login")}
                            >
                                Войти
                            </button>

                            <button
                                type="button"
                                className={`auth_mode_button ${mode === "register" ? "active" : ""}`}
                                onClick={() => switchMode("register")}
                            >
                                Регистрация
                            </button>
                        </div>

                        <div className="auth_modal_title">
                            {mode === "login" ? "Вход" : "Регистрация"}
                        </div>
                    </div>

                    <div className="contect_model_auth__main">
                        <form onSubmit={handleSubmit(onSubmit)}>
                            <label className="default_text">Электронная почта</label>
                            <input
                                {...register('email', { required: true })}
                                className="default_text"
                                type="email"
                                maxLength={250}
                            />

                            <label className="default_text">Пароль</label>
                            <input
                                {...register('password', { required: true })}
                                className="default_text"
                                type="password"
                            />

                            {mode === "register" && (
                                <>
                                    <label className="default_text">Повторите пароль</label>
                                    <input
                                        {...register('confirmPassword', { required: true })}
                                        className="default_text"
                                        type="password"
                                    />
                                </>
                            )}

                            <input
                                className="auth_submit_button"
                                type="submit"
                                disabled={isSubmitting}
                                value={mode === "login" ? "Войти" : "Зарегистрироваться"}
                            />
                        </form>
                    </div>
                </div>
            </Modal>
        </div>
    );
};

export default AuthModelComponent;