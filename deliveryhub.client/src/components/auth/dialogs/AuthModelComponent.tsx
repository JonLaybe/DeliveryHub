import { useEffect, useState, type FC } from "react";
import Modal from "react-modal";
import './AuthModelComponent.scss';

interface AuthModelProps {
    value: boolean;
    onChange: (newValue: boolean) => void;
}

const AuthModelComponent: FC<AuthModelProps> = ({ value, onChange }) => {
    const [modalIsOpen, setModalIsOpen] = useState(value);

    useEffect(() => {
        setModalIsOpen(value);
    }, [value]);

    const openModal = () => {
        setModalIsOpen(true);
        onChange(true);
    }
    const closeModal = () => {
        setModalIsOpen(false);
        onChange(false);
    }

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
                        <form>
                            <label className="default_text">Логин</label>
                            <input className="default_text" type="email" maxLength={250} />
                            <label className="default_text">Пароль</label>
                            <input className="default_text" type="password" />
                            <button>Войти</button>
                        </form>
                    </div>
                </div>
            </Modal >
        </div >
    )
};

export default AuthModelComponent;