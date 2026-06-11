import type { FC } from "react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getCurrentUser, updateProfileAsync } from "../../../services/auth-service/AuthService";
import type { UserDto, UpdateUserDto } from "../../../models/auth-service/UserDto";
import "./ProfileEditComponent.scss";

const ProfileEditComponent: FC = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);
    
    const [formData, setFormData] = useState<UpdateUserDto>({
        firstName: "",
        lastName: "",
        birthDate: "",
        phoneNumber: "",
        country: "",
        city: "",
        photoUrl: ""
    });

    useEffect(() => {
        const fetchUser = async () => {
            try {
                setLoading(true);
                const userData = await getCurrentUser(true);
                setFormData({
                    firstName: userData.firstName || "",
                    lastName: userData.lastName || "",
                    birthDate: userData.birthDate || "",
                    phoneNumber: userData.phoneNumber || "",
                    country: userData.country || "",
                    city: userData.city || "",
                    photoUrl: userData.photoUrl || ""
                });
                setError(null);
            } catch (err) {
                console.error(err);
                setError("Не удалось загрузить данные профиля");
                setTimeout(() => {
                    navigate("/profile");
                }, 2000);
            } finally {
                setLoading(false);
            }
        };
        fetchUser();
    }, [navigate]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
        setSuccess(false);
        setError(null);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSaving(true);
        setError(null);
        setSuccess(false);
        
        try {
            await updateProfileAsync(formData);
            setSuccess(true);
            await getCurrentUser(true);
            
            setTimeout(() => {
                navigate("/profile");
            }, 1500);
        } catch (err: any) {
            console.error(err);
            setError(err.response?.data?.message || "Не удалось обновить данные");
        } finally {
            setSaving(false);
        }
    };

    const formatDateForInput = (dateString?: string) => {
        if (!dateString) return "";
        return dateString.split('T')[0];
    };

    if (loading) {
        return (
            <div className="profile-edit-container">
                <div className="loading">Загрузка...</div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="profile-edit-container">
                <div className="error">{error}</div>
                <div className="loading">Перенаправление...</div>
            </div>
        );
    }

    return (
        <div className="profile-edit-container">
            <div className="profile-edit-card">
                <div className="profile-edit-header">
                    <h1>Редактирование профиля</h1>
                </div>

                {error && (
                    <div className="alert alert-error">
                        {error}
                    </div>
                )}

                {success && (
                    <div className="alert alert-success">
                        ✅ Данные успешно обновлены! Обновляю страницу...
                    </div>
                )}

                <form onSubmit={handleSubmit} className="profile-edit-form">
                    <div className="form-group">
                        <label htmlFor="firstName">Имя</label>
                        <input
                            type="text"
                            id="firstName"
                            name="firstName"
                            value={formData.firstName}
                            onChange={handleChange}
                            placeholder="Введите имя"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="lastName">Фамилия</label>
                        <input
                            type="text"
                            id="lastName"
                            name="lastName"
                            value={formData.lastName}
                            onChange={handleChange}
                            placeholder="Введите фамилию"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="phoneNumber">Телефон</label>
                        <input
                            type="tel"
                            id="phoneNumber"
                            name="phoneNumber"
                            value={formData.phoneNumber}
                            onChange={handleChange}
                            placeholder="+7 (999) 123-45-67"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="birthDate">Дата рождения</label>
                        <input
                            type="date"
                            id="birthDate"
                            name="birthDate"
                            value={formatDateForInput(formData.birthDate)}
                            onChange={handleChange}
                        />
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label htmlFor="country">Страна</label>
                            <input
                                type="text"
                                id="country"
                                name="country"
                                value={formData.country}
                                onChange={handleChange}
                                placeholder="Россия"
                            />
                        </div>

                        <div className="form-group">
                            <label htmlFor="city">Город</label>
                            <input
                                type="text"
                                id="city"
                                name="city"
                                value={formData.city}
                                onChange={handleChange}
                                placeholder="Москва"
                            />
                        </div>
                    </div>

                    <div className="form-group">
                        <label htmlFor="photoUrl">URL фото</label>
                        <input
                            type="url"
                            id="photoUrl"
                            name="photoUrl"
                            value={formData.photoUrl}
                            onChange={handleChange}
                            placeholder="https://example.com/photo.jpg"
                        />
                        {formData.photoUrl && (
                            <div className="photo-preview">
                                <img src={formData.photoUrl} alt="Preview" />
                            </div>
                        )}
                    </div>

                    <div className="profile-edit-actions">
                        <button 
                            type="button" 
                            className="btn btn-secondary" 
                            onClick={() => navigate("/profile")}
                            disabled={saving}
                        >
                            Отмена
                        </button>
                        <button 
                            type="submit" 
                            className="btn btn-primary" 
                            disabled={saving}
                        >
                            {saving ? "Сохранение..." : "Сохранить изменения"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default ProfileEditComponent;