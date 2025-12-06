import { useEffect, useState } from "react";
import { NavLink } from "react-router-dom";
import type { GameInstanceDto } from "@core/types/game";
import { useAtom } from "jotai";
import { activeGamesAtom, gameTemplatesAtom, fetchActiveGamesAtom, fetchGameTemplatesAtom, startGameInstanceAtom } from "@core/atoms/game";
import { authAtom } from "@core/atoms/auth";

export const StartGame: React.FC = () => {
    const [selectedTemplate, setSelectedTemplate] = useState<string>("");
    const [isAutoRepeatable, setIsAutoRepeatable] = useState(false);
    const [drawDate, setDrawDate] = useState("");
    const [drawTime, setDrawTime] = useState("");
    const [expirationDate, setExpirationDate] = useState("");
    const [expirationDayOfWeek, setExpirationDayOfWeek] = useState<number>(6); // Saturday by default
    const [expirationTimeOfDay, setExpirationTimeOfDay] = useState("17:00");

    const [auth,] = useAtom(authAtom);
    const [,startGameInstance] = useAtom(startGameInstanceAtom);
    const [,fetchActiveGames] = useAtom(fetchActiveGamesAtom);
    const [,fetchGameTemplates] = useAtom(fetchGameTemplatesAtom);
    const [templates,] = useAtom(gameTemplatesAtom);
    const [activeGames,] = useAtom(activeGamesAtom);

    useEffect(() => {
        fetchGameTemplates();
        fetchActiveGames();
    }, []);

    // useEffect(() => {
    //     console.log("Active Games Updated:", activeGames);
    //     console.log("Templates Updated:", templates);
        
    //     if (activeGames.length > 0 && templates.length > 0) {
    //         templates.forEach(t => {
    //             const matches = activeGames.filter(g => g.template?.id === t.id);
    //         });
    //     }
    // }, [activeGames, templates]);

    const handleStartGame = async (e: React.FormEvent) => {
        e.preventDefault();
        const gameInstance = {
            createdById: auth?.id || "",
            templateId: selectedTemplate,
            drawDate: new Date(`${drawDate}T${drawTime}`),
            isAutoRepeatable: isAutoRepeatable,
            expirationDateTime: isAutoRepeatable
                ? null
                : new Date(`${expirationDate}T${drawTime}`),
            expirationDayOfWeek: isAutoRepeatable ? expirationDayOfWeek : null,
            expirationTimeOfDay: isAutoRepeatable ? expirationTimeOfDay : null,
        };
        
        await startGameInstance(gameInstance as Partial<GameInstanceDto>).finally(() => {
            fetchActiveGames();
        });
        // Reset form
        setSelectedTemplate("");
        setIsAutoRepeatable(false);
        setDrawDate("");
        setDrawTime("");
        setExpirationDate("");
        setExpirationDayOfWeek(6);
        setExpirationTimeOfDay("17:00");
    };

    // const formatDate = (dateStr: string) => {
    //     const date = new Date(dateStr);
    //     return date.toLocaleDateString("da-DK", { 
    //         day: "2-digit",
    //         month: "2-digit",
    //         year: "numeric",
    //         hour: "2-digit", 
    //         minute: "2-digit",
    //         hour12: false
    //     });
    // };

    // const getStatusColor = (status: string) => {
    //     switch (status) {
    //         case "Active": return "badge-success";
    //         case "Pending Draw": return "badge-warning";
    //         case "Completed": return "badge-info";
    //         default: return "badge-ghost";
    //     }
    // };

    const getGameTypeColor = (type: string) => {
        return type === "Lotto" ? "badge-primary" : "badge-secondary";
    };

    // Filter templates that don't have active games
    const availableTemplates = templates.filter(
        template => !activeGames.some(game => game.template?.id === template.id)
    );
    
    return (
        <div className="container mx-auto px-4 py-6 my-7">
            <div className="space-y-8">
                {/* Header */}
                <div className="flex items-center gap-4 pb-4 border-b-2 border-primary">
                    <div className="flex-1">
                        <h1 className="text-4xl font-bold text-primary">Start Game</h1>
                        <p className="text-base text-base-content/70 mt-1">Create new game instances from templates</p>
                    </div>
                    <NavLink to="/admin/games/overview" className="btn btn-ghost">
                        ← Back to Overview
                    </NavLink>
                </div>

                {/* Available Templates */}
                <div className="card bg-base-200 shadow-lg">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Game Templates</h2>
                        
                        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                            {templates.map((template) => {
                                const hasActiveGame = activeGames.some(
                                    game => game.template?.id === template.id
                                );
                                
                                return (
                                    <div 
                                        key={template.id} 
                                        className={`card bg-base-100 shadow-md ${hasActiveGame ? 'opacity-50' : ''}`}
                                    >
                                        <div className="card-body">
                                            <div className="flex justify-between items-start">
                                                <h3 className="card-title text-lg">{template.name}</h3>
                                                <span className={`badge ${getGameTypeColor(template.gameType)}`}>
                                                    {template.gameType}
                                                </span>
                                            </div>
                                            <p className="text-sm text-base-content/70">{template.description}</p>
                                            
                                            <div className="divider my-2"></div>
                                            
                                            <div className="grid grid-cols-2 gap-2 text-sm">
                                                <div>
                                                    <p className="text-base-content/60">Pool</p>
                                                    <p className="font-semibold">1-{template.poolOfNumbers}</p>
                                                </div>
                                                <div>
                                                    <p className="text-base-content/60">Winning #</p>
                                                    <p className="font-semibold">{template.maxWinningNumbers}</p>
                                                </div>
                                                <div>
                                                    <p className="text-base-content/60">Ticket Range</p>
                                                    <p className="font-semibold">
                                                        {template.minNumbersPerTicket}-{template.maxNumbersPerTicket}
                                                    </p>
                                                </div>
                                                <div>
                                                    <p className="text-base-content/60">Base Price</p>
                                                    <p className="font-semibold">{template.basePrice} DKK</p>
                                                </div>
                                            </div>
                                            
                                            {hasActiveGame && (
                                                <div className="mt-2">
                                                    <div className="badge badge-success badge-sm">Currently Active</div>
                                                </div>
                                            )}
                                        </div>
                                    </div>
                                );
                            })}
                        </div>
                        
                        {templates.length === 0 && (
                            <div className="text-center py-8 text-base-content/60">
                                <p className="text-lg">No templates available</p>
                                <p className="text-sm mb-4">Create a template first to start games</p>
                                <NavLink to="/admin/games/templates/create" className="btn btn-primary btn-sm">
                                    Create Template
                                </NavLink>
                            </div>
                        )}
                    </div>
                </div>

                {/* Start New Game Form */}
                <div className="card bg-base-200 shadow-lg">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Start New Game Instance</h2>
                        
                        <form onSubmit={handleStartGame} className="space-y-4">
                            {/* Template Selection */}
                            <div className="form-control">
                                <label className="label">
                                    <span className="label-text font-medium">Select Game Template</span>
                                    <span className="label-text-alt text-error">*</span>
                                </label>
                                <select 
                                    className="select select-bordered w-full"
                                    value={selectedTemplate}
                                    onChange={(e) => setSelectedTemplate(e.target.value)}
                                    required
                                >
                                    <option value="">Choose a template...</option>
                                    {availableTemplates.map(template => (
                                        <option key={template.id} value={template.id}>
                                            {template.name} ({template.gameType})
                                        </option>
                                    ))}
                                </select>
                                {availableTemplates.length === 0 && (
                                    <label className="label">
                                        <span className="label-text-alt text-warning">
                                            All templates have active games. Complete or end existing games first.
                                        </span>
                                    </label>
                                )}
                            </div>

                            {/* Draw Date & Time */}
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                {/* Draw Date */}
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-medium">Draw Date</span>
                                        <span className="label-text-alt text-error">*</span>
                                    </label>
                                    <input 
                                        type="date"
                                        className="input input-bordered w-full"
                                        value={drawDate}
                                        onChange={(e) => setDrawDate(e.target.value)}
                                        required
                                    />
                                </div>

                                {/* Draw Time */}
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-medium">Draw Time</span>
                                        <span className="label-text-alt text-error">*</span>
                                    </label>
                                    <input 
                                        type="time"
                                        className="input input-bordered w-full"
                                        value={drawTime}
                                        onChange={(e) => setDrawTime(e.target.value)}
                                        step="60"
                                        required
                                    />
                                </div>
                            </div>

                            {/* Auto Repeatable */}
                            <div className="form-control">
                                <label className="label cursor-pointer justify-start gap-4">
                                    <input 
                                        type="checkbox"
                                        className="checkbox checkbox-primary"
                                        checked={isAutoRepeatable}
                                        onChange={(e) => setIsAutoRepeatable(e.target.checked)}
                                    />
                                    <div>
                                        <span className="label-text font-medium">Auto-Repeatable</span>
                                        <p className="text-sm text-base-content/60">
                                            Automatically create a new game instance after this one completes
                                        </p>
                                    </div>
                                </label>
                            </div>

                            {/* Expiration Fields */}
                            {!isAutoRepeatable ? (
                                // Show only expiration date when NOT auto-repeatable
                                <div className="form-control">
                                    <label className="label">
                                        <span className="label-text font-medium">Expiration Date</span>
                                        <span className="label-text-alt text-error">*</span>
                                    </label>
                                    <input 
                                        type="date"
                                        className="input input-bordered w-full"
                                        value={expirationDate}
                                        onChange={(e) => setExpirationDate(e.target.value)}
                                        required
                                    />
                                </div>
                            ) : (
                                // Show day of week and time when auto-repeatable
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-medium">Expiration Day of Week</span>
                                            <span className="label-text-alt text-error">*</span>
                                        </label>
                                        <select 
                                            className="select select-bordered w-full"
                                            value={expirationDayOfWeek}
                                            onChange={(e) => setExpirationDayOfWeek(Number(e.target.value))}
                                            required
                                        >
                                            <option value={0}>Sunday</option>
                                            <option value={1}>Monday</option>
                                            <option value={2}>Tuesday</option>
                                            <option value={3}>Wednesday</option>
                                            <option value={4}>Thursday</option>
                                            <option value={5}>Friday</option>
                                            <option value={6}>Saturday</option>
                                        </select>
                                    </div>

                                    <div className="form-control">
                                        <label className="label">
                                            <span className="label-text font-medium">Expiration Time of Day</span>
                                            <span className="label-text-alt text-error">*</span>
                                        </label>
                                        <input 
                                            type="time"
                                            className="input input-bordered w-full"
                                            value={expirationTimeOfDay}
                                            onChange={(e) => setExpirationTimeOfDay(e.target.value)}
                                            step="60"
                                            required
                                        />
                                    </div>
                                </div>
                            )}

                            {/* Selected Template Info */}
                            {selectedTemplate && (
                                <div className="alert alert-info">
                                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="stroke-current shrink-0 w-6 h-6">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                                    </svg>
                                    <div className="text-sm">
                                        {(() => {
                                            const template = templates.find(t => t.id === selectedTemplate);
                                            return template ? (
                                                <div>
                                                    <p className="font-semibold">{template.name}</p>
                                                    <p>{template.description}</p>
                                                    <p className="mt-1">
                                                        Numbers: 1-{template.poolOfNumbers} | 
                                                        Winning: {template.maxWinningNumbers} | 
                                                        Price: {template.basePrice} DKK
                                                    </p>
                                                </div>
                                            ) : null;
                                        })()}
                                    </div>
                                </div>
                            )}

                            {/* Submit Button */}
                            <div className="flex gap-2 justify-end">
                                <button 
                                    type="submit" 
                                    className="btn btn-primary"
                                    disabled={availableTemplates.length === 0}
                                >
                                    Start Game
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};
