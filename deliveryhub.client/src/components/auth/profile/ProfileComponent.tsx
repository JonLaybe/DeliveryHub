import type { FC } from "react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { logoutAsync } from "../../../services/auth-service/AuthService";
import type { UserDto } from "../../../models/auth-service/UserDto";
import "./ProfileComponent.scss";

const ProfileComponent: FC = () => {
    const navigate = useNavigate();
    const [user, setUser] = useState<UserDto | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchUser = async () => {
            try {
                // const userData = await getCurrentUser(); // временно закомментировано
                const userData: UserDto = {
                    id: "12345",
                    firstName: "Иван",
                    lastName: "Иванов",
                    email: "ivan.ivanov@example.com",
                    birthDate: "1990-01-01",
                    avatarUrl: "https://i.pravatar.cc/150?img=3", // пример аватара
                };
                setUser(userData);
            } catch (err) {
                console.error(err);
            } finally {
                setLoading(false);
            }
        };
        fetchUser();
    }, []);

    const onLogout = async () => {
        await logoutAsync();
        navigate("/", { replace: true });

        // раскомментить, если иконка не будет обновляться сразу
        // window.location.reload();
    };

    if (loading) return <div>Загрузка...</div>;

    return (
        <div className="default_container profile_container">
            <h1>Личный кабинет</h1>
            {user && (
                <div className="profile_info">
                    <img
                        src={user.avatarUrl}
                        alt="Аватар пользователя"
                        className="profile_avatar"
                    />
                    <p><strong>Имя:</strong> {user.firstName}</p>
                    <p><strong>Фамилия:</strong> {user.lastName}</p>
                    <p><strong>Email:</strong> {user.email}</p>
                    <p><strong>Дата рождения:</strong> {user.birthDate}</p>
                </div>
            )}
            <button className="default_text" type="button" onClick={onLogout}>
                Выйти
            </button>
        </div>
    );
};

export default ProfileComponent;