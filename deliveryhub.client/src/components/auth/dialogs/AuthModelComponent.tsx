import { useEffect, useState, type FC } from "react";
import Modal from "react-modal";
import './AuthModelComponent.scss';
import { useForm } from "react-hook-form";
import type { LoginRequestDto, Password } from "../../../models/auth-service/LoginRequestDto";
import { loginAsync, registerAsync } from "../../../services/auth-service/AuthService";                                                   
import { toast } from "react-hot-toast";

interface AuthModelProps {
    value: boolean;
    onChange: (newValue: boolean) => void;
}

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
        setMode("login");
        reset();
    };

    const onLogin = async (dataLoginRequest: LoginRequestDto) => {
        await loginAsync(dataLoginRequest).then(() => closeModal())
        .catch((e) => {
            console.log('>Login failed', e);
            toast.error('Login failed');
            throw 'Login failed';
        });
    };

    type AuthForm = {
        email: string;
        password: Password;
        confirmPassword?: Password;
    };

    const onRegister = async (data: AuthForm) => {
        if ((data.confirmPassword ?? '') !== data.password) {
            console.log('>Passwords do not match');
            throw 'Passwords do not match';
        }

        await registerAsync({ email: data.email, password: data.password })
            .then(() => closeModal())
            .catch(() => {
                console.log('>Register failed');
                throw 'Register failed';
            });
    };

    const onSubmit = async (data: AuthForm) => {
        if (mode === "login") {
            await onLogin({ email: data.email, password: data.password });
        } else {
            await onRegister(data);
        }
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
                        <div className="default_text" style={{ marginTop: 8 }}>
                            {mode === "login" ? "Вход" : "Регистрация"}
                        </div>
                    </div>
                    <div className="contect_model_auth__main">
                        <form onSubmit={handleSubmit(onSubmit)}>
                            <label className="default_text">Логин</label>
                            <input {...register('email')} className="default_text" type="email" maxLength={250} />
                            <label className="default_text">Пароль</label>
                            <input {...register('password')} className="default_text" type="password" />
                            {mode === "register" && (
                                <>
                                    <label className="default_text">Повтори пароль</label>
                                    <input
                                        {...register('confirmPassword')}
                                        className="default_text"
                                        type="password"
                                    />
                                </>
                            )}
                            <div style={{ marginTop: 10 }}>
                                {mode === "login" ? (
                                    <button type="button" className="default_text" onClick={() => setMode("register")}>
                                        Нет аккаунта? Регистрация
                                    </button>
                                ) : (
                                    <button type="button" className="default_text" onClick={() => setMode("login")}>
                                        Уже есть аккаунт? Войти
                                    </button>
                                )}
                            </div>
                            <input
                                type="submit"
                                disabled={isSubmitting}
                                value={mode === "login" ? "Войти" : "Зарегистрироваться"}
                            />

                        </form>
                    </div>
                </div>
            </Modal >
        </div >
    )
};

export default AuthModelComponent;