import type { FC } from "react";
import { useEffect, useState, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { getCurrentUser, logoutAsync } from "../../../services/auth-service/AuthService";
import type { UserDto } from "../../../models/auth-service/UserDto";
import "./ProfileComponent.scss";

const ProfileComponent: FC = () => {
    const navigate = useNavigate();
    const [user, setUser] = useState<UserDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchUser = async () => {
            try {
                setLoading(true);
                const userData = await getCurrentUser();
                setUser(userData);
                console.log("User data:", userData);
            } catch (err) {
                console.error(err);
                setError("Не удалось загрузить данные");
            } finally {
                setLoading(false);
            }
        };
        fetchUser();
    }, []);

    const onLogout = useCallback(async () => {
        await logoutAsync();
        navigate("/", { replace: true });
    }, [navigate]);

    const formatDate = (dateString?: string) => {
        if (!dateString) return "Не указана";
        return new Date(dateString).toLocaleDateString("ru-RU");
    };

    const getDisplayName = () => {
        const firstName = user?.firstName?.trim();
        const lastName = user?.lastName?.trim();
        
        if (firstName && lastName) {
            return `${firstName} ${lastName}`;
        }
        if (firstName) {
            return firstName;
        }
        if (lastName) {
            return lastName;
        }
        return "Пользователь";
    };

    const getInitials = () => {
        const firstName = user?.firstName?.trim();
        const lastName = user?.lastName?.trim();
        
        if (firstName && lastName) {
            return `${firstName[0]}${lastName[0]}`;
        }
        if (firstName) {
            return firstName[0];
        }
        if (lastName) {
            return lastName[0];
        }
        return "П";
    };

    if (loading) {
        return (
            <div className="profile_container">
                <div className="loading">Загрузка...</div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="profile_container">
                <div className="error">{error}</div>
            </div>
        );
    }

    if (!user) {
        return (
            <div className="profile_container">
                <div className="error">Пользователь не найден</div>
            </div>
        );
    }

    return (
        <div className="profile_container">
            <div className="profile_card">
                <div className="profile_header">
                    <div className="avatar">
                        {user.photoUrl ? (
                            <img src={user.photoUrl} alt={getDisplayName()} />
                        ) : (
                            <div className="avatar_placeholder">
                                {getInitials()}
                            </div>
                        )}
                    </div>
                    <div className="profile_title">
                        <h1>{getDisplayName()}</h1>
                        <p className="email">{user.email || "Email не указан"}</p>
                    </div>
                </div>

                <div className="profile_info">
                    <div className="info_row">
                        <span className="label">Телефон:</span>
                        <span className="value">{user.phoneNumber || "Не указан"}</span>
                    </div>
                    <div className="info_row">
                        <span className="label">Страна:</span>
                        <span className="value">{user.country || "Не указана"}</span>
                    </div>
                    <div className="info_row">
                        <span className="label">Город:</span>
                        <span className="value">{user.city || "Не указан"}</span>
                    </div>
                    <div className="info_row">
                        <span className="label">Дата рождения:</span>
                        <span className="value">{formatDate(user.birthDate)}</span>
                    </div>
                    <div className="info_row">
                        <span className="label">ID:</span>
                        <span className="value">{user.id}</span>
                    </div>
                </div>

                <div className="profile_actions">
                    <button className="btn btn_primary" onClick={() => navigate("/profile/edit")}>
                        Редактировать
                    </button>
                    <button className="btn btn_danger" onClick={onLogout}>
                        Выйти
                    </button>
                </div>
            </div>
        </div>
    );
};

export default ProfileComponent;