import type { FC } from "react";
import "./ProfileComponent.scss";
import { useNavigate } from "react-router-dom";
import { logoutAsync } from "../../../services/auth-service/AuthService";

const ProfileComponent: FC = () => {
    const navigate = useNavigate();

    const onLogout = async () => {
        await logoutAsync();
        navigate("/", { replace: true });

        // раскомментить, если иконка не будет обновляться сразу
        // window.location.reload();
    };

    return (
        <div className="default_container profile_container">
            <h1>Profile</h1>

            <button className="default_text" type="button" onClick={onLogout}>
                Выйти
            </button>
        </div>
    );
};

export default ProfileComponent;